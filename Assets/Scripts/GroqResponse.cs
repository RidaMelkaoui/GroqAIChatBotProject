using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroqResponse : MonoBehaviour
{
    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class Choice
    {
        public int index;
        public Message message;
    }

    [System.Serializable]
    public class ChatCompletion
    {
        public string id;
        public string @object;
        public int created;
        public string model;
        public Choice[] choices;
    }

}
