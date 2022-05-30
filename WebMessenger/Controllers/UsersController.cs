using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace WebMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserMemory _userMemory = new();
        private Random rnd = new ();
        private string[] _mails = {"@yahoo.com", "@gmail.com", "@mail.ru"};

        /// <summary>
        /// Generates list of users (length 10).
        /// </summary>
        /// <response code = "200">Successfully generate user.</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("generateUsers")]
        public ActionResult Post()
        {
            List<User> usrs = new List<User>();

            for (int i = 0; i < 10; i++)
            {
                usrs.Add(new User(usrs.Count + 1, GenerateString(true, 5,10), GenerateEmail()));
            }

            try
            {
                _userMemory.SerializeUser(usrs.OrderBy(x => x.Email).ToList());
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// Adds new user to the list.
        /// </summary>
        /// <response code = "200">Successfully add user.</response>
        /// <response code = "400">User with such id already exists. /OR/ Invalid input.</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("addUser")]
        public ActionResult PostNewUsers(User usr)
        {
            try
            {
                _userMemory.SerializeNewUser(usr);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (IOException)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        /// <summary>
        /// Returns user by id.
        /// </summary>
        /// <response code = "200">User with such id was found.</response>
        /// <response code = "400">User with such id was not found. /OR/ Invalid input.</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("getUserById")]
        public ActionResult<User> GetUserById(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                return _userMemory.DeserializeUser(id.Value);
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch(InvalidOperationException)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Returns all users.
        /// </summary>
        /// <response code = "200">User list was successfully found.</response>
        /// <response code = "400">User list was not found.</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("getListOfUsers")]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                return _userMemory.DeserializeUserList();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
        }
        
        /// <summary>
        /// Returns part of the UserList. 
        /// </summary>
        /// <response code = "200">Success.</response>
        /// <response code = "400">Limit or offset is out of range.</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getUsersPartly")]
        public ActionResult<List<User>> GetPartOfList(int limit, int offset)
        {
            if (limit <= 0 || offset < 0) 
            {
                return BadRequest();
            }

            try
            {
                return _userMemory.DeserializeUserList().Skip(offset).Take(limit).ToList();
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
        }

        private string GenerateEmail()
        {
            return GenerateString(false, 8, 16) + rnd.Next(1000, 10000) + _mails[rnd.Next(_mails.Length)];
        }

        private string GenerateString(bool isUp, int min, int max)
        {
            string res = isUp ? Convert.ToString((char)rnd.Next('A', 'Z' + 1)) : "";
            int count = rnd.Next(min, max);
            for (int i = 0; i < count; i++)
                res += (char)rnd.Next('a', 'z' + 1);
            return res;
        }
    }
}