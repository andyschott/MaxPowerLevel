using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MaxPowerLevel.Models;

namespace MaxPowerLevel.Helpers
{
    public class EncounterIterator : IEnumerable<IEnumerable<ItemSlot.SlotHashes>>
    {
        private readonly IEnumerable<Node> _tree;

        public EncounterIterator(PinnacleActivity activity)
        {
            var span = new Span<ItemSlot.SlotHashes[]>(activity.Encounters);
            _tree = CreateTree(span);
        }

        public IEnumerator<IEnumerable<ItemSlot.SlotHashes>> GetEnumerator()
            => new BreadthFirstIterator(_tree);

        private static IEnumerable<Node> CreateTree(Span<ItemSlot.SlotHashes[]> span)
        {
            var nodes = span[0].Select(slot => new Node(slot)).ToList();
            var remaining = span.Slice(1);
            if(!remaining.IsEmpty)
            {
                var children = CreateTree(remaining).ToList();
                foreach(var node in nodes)
                {
                    node.Children.AddRange(children);
                }
            }

            return nodes;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        class Node
        {
            public ItemSlot.SlotHashes Slot { get; }
            public List<Node> Children { get; } = new List<Node>();

            public Node(ItemSlot.SlotHashes slot)
            {
                Slot = slot;
            }

            public override string ToString() => Slot.ToString();
        }

        class BreadthFirstIterator : IEnumerator<IEnumerable<ItemSlot.SlotHashes>>
        {
            private Stack<ItemSlot.SlotHashes> _current = new Stack<ItemSlot.SlotHashes>();
            public IEnumerable<ItemSlot.SlotHashes> Current => _current.Reverse();
            object IEnumerator.Current => Current;

            private readonly IEnumerable<Node> _tree;

            private Stack<IEnumerator<Node>> _enumerators;

            public BreadthFirstIterator(IEnumerable<Node> tree)
            {
                _tree = tree;
                Reset();
            }

            public void Dispose()
            {
                while(_enumerators.Any())
                {
                    _enumerators.Pop().Dispose();
                }
            }

            public bool MoveNext()
            {
                if(_current.Count > 0)
                {
                    _current.Pop();
                }
                return MoveNextImpl();
            }

            private bool MoveNextImpl()
            {
                if(_enumerators.Count == 0)
                {
                    return false;
                }

                var current = _enumerators.Peek();
                current.MoveNext();
                if(current.Current == null)
                {
                    if(_current.Count > 0)
                    {
                        _current.Pop();
                    }
                    
                    _enumerators.Pop();
                    return MoveNextImpl();
                }

                _current.Push(current.Current.Slot);

                if(current.Current.Children.Count == 0)
                {
                    return true;
                }

                var children = current.Current.Children.GetEnumerator();
                _enumerators.Push(children);

                return MoveNextImpl();

            }

            public void Reset()
            {
                _current.Clear();
                _enumerators = new Stack<IEnumerator<Node>>();
                _enumerators.Push(_tree.GetEnumerator());
            }
        }
    }
}