using System.Collections.Generic;
using UnityEngine;

namespace LSG.LWBehaviorTree
{
    public interface IBlackboard
    { }

    public enum EBTState
    {
         DEFAULT, RUNNING, SUCCESS, FAILED
    }

    public interface INode
    {
        EBTState Update(IBlackboard bb);
    }

    public interface IBegins
    {
        void OnBegin(IBlackboard bb);
    }

    public sealed class RepeatNode : CompositeNode
    {
        public override EBTState Update(IBlackboard bb)
        {
            for (int i = runningNode, len = children.Count; i < len; ++i)
            {
                switch (children[i].Update(bb))
                {
                    case EBTState.RUNNING:
                        runningNode = i;
                        return EBTState.RUNNING;
                    case EBTState.FAILED:
                        continue;
                }
            }

            Initialize();
            return EBTState.SUCCESS;
        }
    }


    /// <summary>
    /// negate result
    /// have a Child 
    /// </summary>
    public sealed class DecoratorNode : INode
    {
        INode child;

        public DecoratorNode(INode node)
        {
            child = node;
        }

        public EBTState Update(IBlackboard bb)
        {
            switch(child.Update(bb))
            {
                case EBTState.FAILED:
                    return EBTState.SUCCESS;
                case EBTState.SUCCESS:
                    return EBTState.FAILED;
            }
            return EBTState.RUNNING;
        }
    }

    public abstract class CompositeNode : INode
    {
        protected List<INode> children;
        protected int runningNode;

        protected CompositeNode()
        {
            children = new List<INode>();
            Initialize();
        }

        public virtual void Initialize()
        {
            runningNode = 0;
        }

        public void Add(INode node)
        {
            children.Add(node);
        }

        public abstract EBTState Update(IBlackboard bb);
    }

    /// <summary>
    /// 자식 들 중 하나라도 결과가 성공 또는 러닝 상태 이며 동작 중단
    /// </summary>
    public sealed class SelectNode : CompositeNode
    {
        public override EBTState Update(IBlackboard bb)
        {
            for(int i = runningNode, len = children.Count; i < len; ++i)
            {
                switch(children[i].Update(bb))
                {
                    case EBTState.RUNNING:
                        runningNode = i;
                        break;
                    case EBTState.SUCCESS:
                        Initialize();
                        return EBTState.SUCCESS;
                    default:
                        continue;
                }
            }

            Initialize();
            return EBTState.FAILED;
        }
    }

    /// <summary>
    /// 자식 들 중 결과가 실패 이거나 러닝 상태 이면 중단
    /// </summary>
    public sealed class SequenceNode : CompositeNode
    {
        public override EBTState Update(IBlackboard bb)
        {
            for (int i = runningNode, len = children.Count; i < len;)
            {
                switch (children[i].Update(bb))
                {
                    case EBTState.RUNNING:
                        runningNode = i;
                        return EBTState.RUNNING;
                    case EBTState.SUCCESS:
                        ++i;
                        break;
                    case EBTState.FAILED:
                        Initialize();
                        return EBTState.FAILED;
                }
            }

            Initialize();
            return EBTState.SUCCESS;
        }
    }

    public abstract class ActionNode : INode, IBegins
    {
        public abstract void OnBegin(IBlackboard bb);
        public abstract EBTState Update(IBlackboard bb);
    }

    public struct Condition :INode, IBegins
    {
        public delegate EBTState CallbackUpdate(IBlackboard bb);
        public delegate void CallbackOnBigins(IBlackboard bb);

        public CallbackUpdate onUpdate;
        public CallbackOnBigins onBeigins;

        public void OnBegin(IBlackboard bb)
        {
            if (onBeigins != null)
                onBeigins.Invoke(bb);
        }

        public EBTState Update(IBlackboard bb)
        {
            if (onUpdate == null)
                return EBTState.SUCCESS;
            
            return onUpdate.Invoke(bb);
        }
    }
}