using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EBunnyChat
{
	public class ChatClient : IDisposable
	{
		private static readonly Lazy<ChatClient> LazyInstance = new Lazy<ChatClient>(() => new ChatClient());
		private readonly IFormatter _formatter = new BinaryFormatter();
		private IModel _channel;
		private IConnection _connection;
		private Thread _consumerThread;
		private volatile bool _shouldStop;

		private ChatClient()
		{
		}

		private string Server { get; set; }
		public string LocalUserEmail { get; set; }
		public string LocalUserName { get; set; }

		public static ChatClient Instance
		{
			get { return LazyInstance.Value; }
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			Stop();
		}

		#endregion

		public event EventHandler<ChatMessage> MessageArrived;

		public void Start()
		{
			//ask for credentials

			// set-up connection
			var factory = new ConnectionFactory {HostName = Server};

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			_channel.ExchangeDeclare("e-bunny-chat", "fanout");

			// sign-up for incoming messages
			var queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(queueName, "e-bunny-chat", "");
			var consumer = new QueueingBasicConsumer(_channel);
			_channel.BasicConsume(queueName, true, consumer);

			_consumerThread = new Thread(ConsumerThreadWorker);
			_consumerThread.Start(consumer);
		}

		private void ConsumerThreadWorker(object obj)
		{
			var consumer = (IQueueingBasicConsumer) obj;

			while (!_shouldStop)
			{
				BasicDeliverEventArgs ea;

				if (consumer.Queue.Dequeue(100, out ea))
				{
					var body = ea.Body;
					var message = DeserializeChatMessage(body);

					OnMessageArrived(message);
				}
			}
		}

		private void RequestStop()
		{
			_shouldStop = true;
		}

		public void Stop()
		{
			RequestStop();
			_consumerThread.Join();

			if (_channel != null) _channel.Dispose();
			if (_connection != null) _connection.Dispose();
		}

		public void SendMessage(ChatMessage message)
		{
			if (!_channel.IsOpen)
				return;

			_channel.BasicPublish("e-bunny-chat", "", null, SerializeChatMessage(message));
		}

		private byte[] SerializeChatMessage(ChatMessage message)
		{
			byte[] buffer;
			using (var ms = new MemoryStream())
			{
				_formatter.Serialize(ms, message);
				buffer = ms.ToArray();
			}

			return buffer;
		}

		private ChatMessage DeserializeChatMessage(byte[] buffer)
		{
			ChatMessage message;

			using (var ms = new MemoryStream(buffer))
			{
				message = (ChatMessage) _formatter.Deserialize(ms);
			}

			return message;
		}

		protected virtual void OnMessageArrived(ChatMessage e)
		{
			var handler = MessageArrived;
			if (handler != null) handler(this, e);
		}

		public void Config(string server, string name, string email)
		{
			Server = server;
			LocalUserName = name;
			LocalUserEmail = email;
		}
	}
}