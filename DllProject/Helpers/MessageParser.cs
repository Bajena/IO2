using System;
using System.Collections.Generic;

namespace DllProject.Helpers
{
    public class MessageParser
    {
        private Queue<string> _messagesToProcess;
        string _lastIncompleteMessage;

        private XmlValidator.XmlValidator _xmlValidator;

        public MessageParser()
        {
            _xmlValidator = new XmlValidator.XmlValidator();
            _messagesToProcess = new Queue<string>();
        }

        /// <summary>
        /// Returns and dequeues complete messages from unprocessed messages queue
        /// </summary>
        /// <returns>Unprocessed messages</returns>
        public List<string> GetMessagesToProcess()
        {
            var list = new List<string>();
            if (_messagesToProcess.Count == 0) return list;

            while(_messagesToProcess.Count!=0)
                list.Add(_messagesToProcess.Dequeue());
            
            return list;
        }

        /// <summary>
        /// Parses string for xml messages and adds them to queue
        /// </summary>
        /// <param name="messagesString"></param>
        public void ParseMessage(string messagesString)
        {

            var splitMessages = messagesString.Split(new[] { "<?xml" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < splitMessages.Length; i++)
            {
                if (i == 0 && _lastIncompleteMessage!=null)
                {
                    var completeMessage = _lastIncompleteMessage + splitMessages[0];
                    _messagesToProcess.Enqueue(completeMessage);
                    _lastIncompleteMessage = null;
                }
                else
                {
                    splitMessages[i] = "<?xml" + splitMessages[i];
                    if (i == splitMessages.Length - 1 && !_xmlValidator.IsValidXml(splitMessages[i]))
                    {
                        _lastIncompleteMessage = splitMessages[i];
                    }
                    else
                    {
                        _messagesToProcess.Enqueue(splitMessages[i]);
                    }
                }
            }

        }


    }
}
