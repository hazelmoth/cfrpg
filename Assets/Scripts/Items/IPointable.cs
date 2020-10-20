
using UnityEngine;

namespace Items
{
    // An item that can be held in actors' hands and visibly pointed at things
    public interface IAimable
    {
        Sprite heldItemSprite { get; }
        Direction pointDirection { get; }
    }
}