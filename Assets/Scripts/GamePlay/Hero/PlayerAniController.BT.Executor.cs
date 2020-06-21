using UnityEngine;
using System.Collections.Generic;
namespace LSG.BT
{

    public partial class PlayerAniController
    {
        enum EBTState
        {
            RUNNING, SUCCESS, FAILED
        }

        internal abstract class Node
        {

        }

        internal class Sequence : Decorator
        {
            List<Node> children;
        }

        internal class Selector : Decorator
        {
            List<Node> children;
        }

        internal class Decorator : Node
        {
            Node child;
        }

        internal class Task : Node
        {

        }
    }
}


