using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An event for calling the game object send message method.
    /// Cannot be reversed.
    /// </summary>
    [CutsceneItemAttribute("Game Object", "Send Message", CutsceneItemGenre.ActorItem)]
    public class SendMessageGameObject : CinemaActorEvent
    {
        public enum SendMessageValueType
        {
            None,
            Int,
            //Long,
            Float,
            Double,
            Bool,
            String,
        }

        public string MethodName = string.Empty;
        public object Parameter = null;
        public SendMessageValueType ParameterType = SendMessageValueType.Int;
        public SendMessageOptions SendMessageOptions = SendMessageOptions.DontRequireReceiver;

        public int intValue = 0;
        //public long longValue = 0L;
        public float floatValue = 0.0F;
        public double doubleValue = 0.0;
        public bool boolValue = false;
        public string stringValue = string.Empty;

        /// <summary>
        /// Trigger this event and send the message.
        /// </summary>
        /// <param name="actor">the actor to send the message to.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                // Set parameter with selected value
                switch (ParameterType)
                {
                    case SendMessageValueType.Int:
                        Parameter = intValue;
                        break;
                    //case SendMessageValueType.Long:
                    //    Parameter = longValue;
                    //    break;
                    case SendMessageValueType.Float:
                        Parameter = floatValue;
                        break;
                    case SendMessageValueType.Double:
                        Parameter = doubleValue;
                        break;
                    case SendMessageValueType.Bool:
                        Parameter = boolValue;
                        break;
                    case SendMessageValueType.String:
                        Parameter = stringValue;
                        break;
                    default:
                        Parameter = null;
                        break;
                }

                actor.SendMessage(MethodName, Parameter, SendMessageOptions);
            }
        }

    }
}