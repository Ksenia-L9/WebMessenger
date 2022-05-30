using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Memory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Model;

namespace WebMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private MessageMemory _messMemory = new MessageMemory();
        private Random rnd = new Random();
        
        /// <summary>
        /// Generates list of messages (length 100).
        /// </summary>
        /// <response code = "200">Successfully generate messages.</response>
        /// <response code = "500">If server can not connect the database.</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("generateMessages")]
        public ActionResult Post()
        {
            List<Message> mess = new List<Message>();
            
            for (int i = 0; i < 100; i++)
            {
                mess.Add(new Message(GenerateString(true, 3, 6), GenerateString(true, 8, 15), 
                    rnd.Next(1, 11), rnd.Next(1, 11)));
            }

            try
            {
                _messMemory.SerializeMessages(mess);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// Returns messages by sender and receiver id.
        /// </summary>
        /// <response code = "200">Messages were successfully found. /OR/ Messages with such ids were not found.</response>
        /// <response code = "400">Invalid input.</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("getMessagesByBothId")]
        public ActionResult<List<Message>> GetMessagesByIds(int? idSent, int? idReceived)
        {
            if (idSent == null || idReceived == null)
            {
                return BadRequest();
            }

            try
            {
                return _messMemory.DeserializeMessages(idSent.Value, idReceived.Value);
            }
            catch (ArgumentNullException)
            {
                return Ok("No messages was found.");
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Returns messages by sender id.
        /// </summary>
        /// <response code = "200">Messages were successfully found. /OR/ Messages with such ids were not found.</response>
        /// <response code = "400">Invalid input.</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("getMessagesBySenderId")]
        public ActionResult<List<Message>> GetMessagesBySenderId(int? idSent)
        {
            if (idSent == null)
            {
                return BadRequest();
            }

            try
            {
                return _messMemory.DeserializeMessagesBySenderId(idSent.Value);
            }
            catch (ArgumentNullException)
            {
                return Ok("No messages was found.");
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
        }
        
        /// <summary>
        /// Returns messages by receiver id.
        /// </summary>
        /// <response code = "200">Messages were successfully found. /OR/ Messages with such ids were not found.</response>
        /// <response code = "400">Invalid input.</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("getMessageByReceiverId")]
        public ActionResult<List<Message>> GetMessagesByReceiverId(int? idReceived)
        {
            if (idReceived == null)
            {
                return BadRequest();
            }

            try
            {
                return _messMemory.DeserializeMessagesByReceiverId(idReceived.Value);
            }
            catch (ArgumentNullException)
            {
                return Ok("No messages was found.");
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Add new message to the list.
        /// </summary>
        /// <response code = "200">Message was successfully added. /OR/ Messages with such ids were not found.</response>
        /// <response code = "400">Invalid input /OR/ There is no user with such id (for sending/receiving).</response>
        /// <response code = "500">If server can not connect the database (json-file).</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("addMessage")]
        public ActionResult PostNewMessage(Message mes)
        {
            try
            {
                _messMemory.SerializeAddedMessage(mes);
            }
            catch (IOException)
            {
                return StatusCode(500);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }

            return Ok();
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