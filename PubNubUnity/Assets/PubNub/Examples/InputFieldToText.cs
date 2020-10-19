using UnityEngine;
using System;
using UnityEngine.UI;   

namespace PubNubExample
{
    public class InputFieldToText : MonoBehaviour {
        public InputField Field;
        public Text TextBox;

        public void CopyText() {
            //TextBox.text = Field.text;
        }
    }
}