using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model;
using Newtonsoft.Json;

namespace Memory
{
    public class MessageMemory
    {
        private readonly string path = "messages.json";

        /// <summary>
        /// Saves list of messages to json file. 
        /// </summary>
        public void SerializeMessages(List<Message> messages)
        {
            using StreamWriter sw = new StreamWriter(path);
            sw.Write(JsonConvert.SerializeObject(messages));
        }

        /// <summary>
        ///  Saves added message to the file.
        /// </summary>
        public void SerializeAddedMessage(Message mes)
        {
            using StreamReader sr = new StreamReader(UserMemory.path);
            List<User> users = JsonConvert.DeserializeObject<List<User>>(sr.ReadLine());
            
            using StreamReader sr1 = new StreamReader(path);
            List<Message> messages = JsonConvert.DeserializeObject<List<Message>>(sr1.ReadLine());

            if (!users.Exists(usr => usr.Id == mes.SenderId) || !users.Exists(usr => usr.Id == mes.ReceiverId))
                throw new ArgumentException();
            
            messages.Add(mes);
            
            using StreamWriter sw = new StreamWriter(path);
            sw.Write(JsonConvert.SerializeObject(messages));
        }
        
        
        /// <summary>
        /// Uploads messages from the file.
        /// </summary>
        public List<Message> DeserializeMessages(int senderId, int receiverId)
        {
            using StreamReader sr = new StreamReader(path);
            return JsonConvert.DeserializeObject<List<Message>>(sr.ReadLine())
                    .FindAll(x => x.ReceiverId == receiverId && x.SenderId == senderId);
        }

        /// <summary>
        /// Uploads messages by sender id.
        /// </summary>
        public List<Message> DeserializeMessagesBySenderId(int idSent)
        {

            using StreamReader sr = new StreamReader(path);
            return JsonConvert.DeserializeObject<List<Message>>(sr.ReadLine()).FindAll(x => x.SenderId == idSent);
        }
        
        /// <summary>
        /// Uploads messages by receiver id.
        /// </summary>
        public List<Message> DeserializeMessagesByReceiverId(int idReceived)
        {

            using StreamReader sr = new StreamReader(path);
            return JsonConvert.DeserializeObject<List<Message>>(sr.ReadLine()).FindAll(x => x.ReceiverId == idReceived);
        }
    }
}