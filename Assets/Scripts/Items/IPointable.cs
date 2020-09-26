
using UnityEngine;

namespace Items
{
    // An item that can be held in actors' hands and visibly pointed at things
    public interface IPointable
    {
        Sprite heldItemSprite { get; }
        Direction pointDirection { get; }
    }
}