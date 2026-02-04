using UnityEngine;

namespace Entities.Enemies
{
    [RequireComponent(typeof(Animator))]
    public class Enemy : Entity
    {
        [SerializeField] private Animator _animator;
    }
}