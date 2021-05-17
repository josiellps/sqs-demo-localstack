using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aws_sqs_demo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static string SecretKey = "ignore";
        private static string AccessKey = "ignore";
        private static string ServiceUrl = "http://localhost:4576";
        private static string QueueName = "myQueue";
        private static string QueueUrl = "http://localhost:4576/0000000000/myQueue";
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                  CreateQueue();
                //SendMessage($"Mensagem das horas {DateTime.Now.ToString()}");
                await Task.Delay(1000, stoppingToken);
            }
        }

        private static void CreateQueue()
        {
            var awsCreds = new BasicAWSCredentials(AccessKey, SecretKey);
            var config = new AmazonSQSConfig
            {
                ServiceURL = ServiceUrl,
                //RegionEndpoint = RegionEndpoint.USEast1
            };
            var amazonSqsClient = new AmazonSQSClient(awsCreds, config);
            var createQueueRequest = new CreateQueueRequest();
            try
            {
                amazonSqsClient.GetQueueUrlAsync(QueueName);
                var result = amazonSqsClient.ListQueuesAsync(QueueName);
                if (result.Result.QueueUrls.Count <= 0)
                {
                    try
                    {
                        var lista = amazonSqsClient.GetQueueUrlAsync(QueueName);

                        // create channel
                        createQueueRequest.QueueName = QueueName;
                        var createQueueResponse = amazonSqsClient.CreateQueueAsync(createQueueRequest);
                        Console.WriteLine("QueueUrl : " + createQueueResponse.Result.QueueUrl);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string SendMessage(string message)
        {
            var awsCreds = new BasicAWSCredentials(AccessKey, SecretKey);
            var config = new AmazonSQSConfig
            {
                ServiceURL = ServiceUrl,
                //RegionEndpoint = RegionEndpoint.USEast1
            };
            var amazonSqsClient = new AmazonSQSClient(awsCreds, config);
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = QueueUrl,
                MessageBody = message
            };
            var sendMessageResponse = amazonSqsClient.SendMessageAsync(sendMessageRequest);
            return sendMessageResponse.Result.SequenceNumber;
        }
    }
}
