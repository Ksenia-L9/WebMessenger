using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model;
using Newtonsoft.Json;

namespace Memory
{
    public class UserMemory
    {
        public static string path = "users.json";
        
        /// <summary>
        /// Saves list of users.
        /// </summary>
        public void SerializeUser(List<User> users)
        {
            using StreamWriter sw = new StreamWriter(path);
            sw.Write(JsonConvert.SerializeObject(users));
        }

        /// <summary>
        /// Saves new user to the list.
        /// </summary>
        public void SerializeNewUser(User usr)
        {
            using StreamReader sr = new StreamReader(path);
            List<User> users = JsonConvert.DeserializeObject<List<User>>(sr.ReadLine());
            
            if (usr.Id <= users.Count || users.Exists(u=> u.Email == usr.Email))
                throw new ArgumentException();
            
            users.Add(usr);
            using StreamWriter sw = new StreamWriter(path);
            sw.Write(JsonConvert.SerializeObject(users));
        }

        /// <summary>
        /// Uploads user by id from the file.
        /// </summary>
        public User DeserializeUser(int id)
        {
            using StreamReader sr = new StreamReader(path);
            return JsonConvert.DeserializeObject<List<User>>(sr.ReadLine()).Single(usr => usr.Id == id);
        }

        /// <summary>
        /// Uploads all users from the file.
        /// </summary>
        public List<User> DeserializeUserList()
        {
            using StreamReader sr = new StreamReader(path);
            return JsonConvert.DeserializeObject<List<User>>(sr.ReadLine());
        }
    }
}