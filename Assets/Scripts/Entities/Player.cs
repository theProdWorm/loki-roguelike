using Abilities;
using Abilities.Attacks;
using Stats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Entities.Stats;
using Gameplay.Input;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Entities
{
    /*⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠟⠛⠉⠁⠀⠀⠀⠀⠈⠉⠉⠙⠛⠛⠿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠛⠉⠁⠀⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠻⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠀⠀⠀⢲⣶⣶⣷⣶⣶⣦⣤⣀⠀⠀⠀⠀⠀⠀⠈⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠋⠀⠀⠀⠀⠀⠀⠀⢀⣤⣶⣿⣿⣿⣷⣆⠀⢻⣿⣿⣿⣿⣿⣿⣿⣿⣷⣦⣄⠀⠀⠀⠀⠈⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⡀⠘⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣄⠀⠀⠀⠀⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠉⠻⢿⣿⣿⣿⡿⠛⠀⠀⠀⠀⠀⠠⠀⠀⠀⣼⠀⡿⠛⠛⠙⠛⠛⠛⠛⢧⠀⠘⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧⡀⠀⠀⠀⠘⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⢹⡿⠋⠀⠀⠀⠀⠀⠀⠀⠂⠀⠀⠘⠇⠀⢀⣤⣴⣶⣶⣦⣤⡀⠀⢠⡀⠘⣿⣿⡟⠁⣹⣿⣿⣿⣿⣿⣿⣿⣿⣄⠀⠀⠀⠘⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⠁⠠⠁⠀⠈⠀⢠⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⢳⡀⠈⠋⢀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡆⠀⠀⠀⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⢀⠠⢁⠂⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⡏⠀⢸⣷⣄⣴⣿⣿⣿⡟⠁⣽⣿⣿⣿⣿⣿⣿⣧⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡐⠀⡈⢀⠐⡀⠂⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⠇⠀⣼⣿⣿⣿⣿⣿⠏⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠂⠀⠐⠀⠄⠂⢄⡁⠀⠀⠀⠸⣿⣿⣿⠿⢿⣿⣿⠟⠀⣰⣿⣿⣿⣿⡿⠁⠀⠚⠛⠛⠛⠛⠛⠛⠛⠛⠿⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠄⡁⢆⠂⠀⠀⠀⠀⠙⠻⠿⠦⠶⠛⠋⢀⣴⣿⣿⣿⣿⣿⠗⠀⠀⠀⠀⠀⠒⠶⣶⣶⣶⣦⡄⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠎⡑⢢⠄⠀⠀⠁⢂⠔⡨⠀⠀⠀⠀⠀⠖⠆⠀⠀⠀⢀⣴⣿⣿⣿⣿⣿⡟⠁⠀⢀⣴⣾⣿⣷⣦⡀⠈⠻⣿⣿⡇⠀⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠈⢑⠢⡁⢰⡀⠀⠈⢆⡑⠀⠀⠀⠀⠀⢠⣄⣀⣴⡾⠟⠛⠋⠉⠉⣿⣿⠀⠀⢠⣿⣿⣿⣿⣿⣿⣿⡄⠀⢸⡿⠀⠀⠀⠀⢰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⢀⢀⣀⡀⠄⠀⠀⢈⠶⣵⣿⡇⠀⢈⠆⠀⠀⠀⣠⡆⠀⠀⣿⣿⣿⣤⣀⣠⣴⡆⠀⣿⡇⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⡄⠈⠁⠀⠀⠀⠀⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠌⠂⠀⠀⠀⠀⠀⡈⠆⣿⠟⠀⠀⡐⠈⠀⠀⠀⠻⢁⠀⠀⢸⣿⣿⣿⣿⣿⣿⣧⣀⣿⡇⠀⠀⣿⡟⠿⣿⣿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠠⠀⠔⠁⠀⠀⠀⠀⠀⠀⠀⠀⠠⣿⠃⡀⠀⠙⠻⢿⣿⣿⣿⣿⣿⣿⣇⠀⠀⠈⠲⢴⣿⣿⡿⠿⠛⠁⠀⠀⠀⢀⠂⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠂⠀⠀⠀⠀⠀⠁⣴⣿⣶⣤⠀⠀⠉⠻⣿⣿⣿⣿⣿⡀⢱⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡈⠄⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⣾⠇⠀⠀⠀⠀⠀⠀⠀⠀⠙⢿⠋⣰⣷⣤⣀⠀⠉⠛⠿⣿⣿⡶⠿⠛⠒⠀⠀⢀⣤⡖⠀⠀⠐⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⡏⠀⢀⣄⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⠿⡿⢉⣴⠂⢀⣄⡀⠀⠀⠀⠀⢠⠦⠄⠼⠿⠏⠀⠀⠁⠀⠀⠀⠀⠀⠹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⠀⢀⣾⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠃⠴⠿⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠀⠈⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠐⡀⠀⠀⠀⠀⠀⠀⢸⡿⠀⠈⣡⡦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠀⠀⠀⠀⠀⠘⣆⠀⠀⠀⠀⠀⠀⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠘⡄⠀⠀⠀⠀⠀⠀⢸⡷⠀⠸⠋⠁⣠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⠟⠁⠀⠀⠀⠀⠀⠀⢀⣀⡀⠀⠀⠀⠀⡀⠀⠀⠀⢹⣷⡀⠀⠀⠀⠀⠀⠙⣿⣿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠌⡰⠀⠀⠀⠀⠀⠀⢸⣿⠀⠀⠰⠞⠁⣴⡷⠀⠀⠀⢀⡼⠃⢀⣴⠞⠁⠀⠀⠀⠀⠂⠄⠀⠀⢸⣿⣿⣷⣶⣤⣄⣷⡀⠀⠀⠀⢻⣷⣄⠀⠀⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⣿
      ⠀⠂⠄⠣⠄⠀⠀⠀⠀⢸⣿⣷⣄⠀⠀⠘⠋⢀⣴⣷⠀⠛⢁⣴⠟⠁⠀⠀⠀⠄⠀⠠⢁⠂⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣧⠀⠀⠀⠀⢿⣿⣦⠀⠀⠀⢁⠀⠈⠻⣿⣿⣿⣿⣿⣿
      ⠀⠈⠄⠃⡜⢡⠀⠀⠀⠀⠛⢿⣿⣷⣄⡐⠀⠈⠛⠃⠀⣰⡿⠁⠀⠀⠀⠠⢈⠀⠀⠐⠠⢈⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣇⠀⠀⠀⠈⢿⣿⣷⡀⠀⠀⠐⡀⠀⠙⣿⣿⣿⣿⣿
      ⠀⠈⠄⠡⢀⢃⠒⡄⠀⠀⠀⠀⠉⠻⢿⣿⣷⣦⣄⣀⣼⠋⠀⠀⠀⠀⠌⡐⠀⠀⠀⠌⡐⠁⠀⠀⣰⣿⣿⡿⠛⠁⠀⠀⠉⠻⠄⠀⠀⠀⠈⢿⣿⣿⣄⠀⠀⠠⢁⠀⠘⢿⣿⣿⣿
      ⠀⠀⠌⡐⠠⢈⠒⡈⢆⡀⠀⠀⠀⠀⠀⠈⠛⢿⣿⡟⠀⠀⠀⠀⠄⡁⠂⠀⠀⠀⠀⠀⠀⠀⠀⠰⣿⡿⠋⠀⢀⣴⣿⣿⣶⠄⠀⠀⠀⠀⠀⠈⢿⣿⣿⣧⡀⠀⠀⠆⡀⠈⢿⣿⣿
      ⠀⠀⠀⠀⠀⠂⠐⠀⠂⠀⠁⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⠀⠄⡁⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠟⠀⠀⡰⢌⡾⠛⠉⠀⠀⠘⠦⠀⠀⠀⠀⠈⢿⣿⣿⣷⡄⠀⠠⢁⠀⠀⢻⣿
      ⠠⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⠡⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠰⣈⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢻⣿⣿⣿⣆⠀⠀⠌⡀⠀⢻
      ⠀⠂⠌⡐⠄⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠠⢀⠂⠌⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠛⠿⣿⣿⣆⠀⠐⢀⠂⠀
      ⠀⠌⡐⠀⠀⠀⠈⠀⠀⠀⠀⠀⠠⢁⠂⠤⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢃⠖⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⢿⣧⡀⠀⠘⡀
      ⠀⠀⠀⠈⠀⠁⠀⠀⠀⠀⠀⠈⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢨⠎⡑⢆⣤⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠷⣄⠀⠐
      ⣿⣾⣶⣷⣾⣿⣿⣿⣿⣿⣿⣿⡟⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠊⠵⠯⠞⠛⠒⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠄⠀⠀⢀⠀⠈⠓⠀
      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣷⣤⣀⣀⠀⠀⠀⠀⣠⣿⣿⣶⣤⡀⠀⠀⠀⠀⠀⠀⢈⠀⠀⠀⠄⠠⠀
      ⣿⣿⣿⣿⣿⣿⣿⡿⠟⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠓⠒⠛⠛⠛⠉⠀⠀⢀⣠⣾⣿⣿⣿⣿⣿⣿⣶⣄⡀⠀⠀⠀⠀⠀⠂⠀⠀⠁⠄*/
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Entity
    {
        public enum Character { Fenrir, Hel }

        private static readonly int IS_MOVING = Animator.StringToHash("isMoving");
        private static readonly int DASH = Animator.StringToHash("dash");
        private static readonly int ATTACK = Animator.StringToHash("attack");
        private static readonly int SWITCH = Animator.StringToHash("switch");

        public UnityEvent<int, int> OnHealthChanged;
        public UnityEvent<int, int> OnPotionChargesChanged;
        public UnityEvent OnPotionDrunk;
        public UnityEvent OnDashStarted;
        public UnityEvent OnDashFinished;

        [SerializeField] private Transform _characterContainer;
        [SerializeField] private PlayerInput _playerInput;

        [Tooltip("Amount of time (in seconds) in advance the player can press an input for it to count.")]
        [SerializeField] private float _inputBufferMargin;

        [SerializeField] private float _lowHealthThreshold;

        [Header("Movement")]
        [SerializeField] private float _animationLockMoveSpeedFadeDuration;

        [Header("Target Lock")]
        [SerializeField] private float _targetLockAngle;

        [SerializeField] private float _targetLockMaxDistance;
        [SerializeField] private float _targetLockAngleWeight;
        [SerializeField] private float _targetLockDistanceWeight;

        [Header("Collision")]
        [SerializeField] private CapsuleCollider _collider;

        [SerializeField] private LayerMask _wallLayer;
        [SerializeField] private LayerMask _holeLayer;

        [Header("Interaction")]
        [SerializeField] private float _lookWeight;

        [SerializeField] private float _distanceWeight;

        [Header("Healing")]
        [SerializeField] private int _potionHealAmount;
        [SerializeField] private int _potionCost;
        [SerializeField] private int _maxPotionCharges;

        [Header("Dash")]
        [SerializeField] private Ability _dashAbility;

        [SerializeField] private Transform _dashPoint;

        [Range(0.02f, 0.5f)]
        [SerializeField] private float _dashDuration;

        [Range(0f, 1f), Tooltip("Fraction of dash duration to fade back to normal speed.")]
        [SerializeField] private float _dashFade;

        [Tooltip("The fraction cutoff for dashing OVER holes")]
        [Range(0.5f, 1f)]
        [SerializeField] private float _dashHoleSnapFraction;

        [SerializeField] private LayerMask _dashingPlayerLayer;

        [Header("Fenrir")]
        [SerializeField] private CharacterAbilitySet _fenrirAbilities;
        [SerializeField] private Animator _fenrirAnimator;
        [SerializeField] private Transform _fenrirAttackPoint;
        [SerializeField] private float _fenrirLungeForce;
        [SerializeField] private float _fenrirLungeDuration;
        
        [Header("Hel")]
        [SerializeField] private CharacterAbilitySet _helAbilities;
        [SerializeField] private Animator _helAnimator;
        [SerializeField] private Transform _helAttackPoint;
        [SerializeField] private float _helLungeForce;
        [SerializeField] private float _helLungeDuration;

        [Header("Freeze")]
        [SerializeField] public int   ShatterBonusDamage = 20;
        [SerializeField] public float HelFreezeDamageMultiplier = 0.5f;
        
        private Animator[] _animators;
        private Animator CurrentAnimator => _animators[(int)ActiveCharacter];

        public Character ActiveCharacter;

        private AttackAbilityTracker[] _attackAbilityTrackers;
        private AttackAbilityTracker[] _switchAbilityTrackers;
        private AbilityTracker _dashAbilityTracker;
        private AttackAbilityTracker AttackAbilityTracker => _attackAbilityTrackers[(int)ActiveCharacter];
        private AttackAbilityTracker SwitchAbilityTracker => _switchAbilityTrackers[(int)ActiveCharacter];

        private Transform[] _attackPoints;

        private Vector3 AttackPosition => _attackPoints[(int)ActiveCharacter].position;

        private Ability _currentAbility;
        private int _currentAbilityUseTimes;

        private PlayerBaseStats _playerBaseStats;

        private Camera _camera;

        private Vector2 _moveInput;

        private float _critChance;
        private float _critDamage;

        private float _damageReduction = 0f;

        private int _potionCharges;
        private bool PotionReady => _potionCharges >= _potionCost;

        private float _originalDashDistance;
        private float _originalMoveSpeed;
        private Coroutine _dashCoroutine;

        private Vector3 _targetPos;

        private bool _isDashing;
        private bool _hasControl = true;
        private float _controlLossDuration;

        private Coroutine _lungeCoroutine;
        private Vector3 _lungeForce;

        private InputBuffer _inputBuffer;

        private List<IInteractable> _interactables = new();
        private IInteractable _currentInteractable;

        private StatsPersistence _statsPersistence;

        protected override void Start()
        {
            #region StatsPersistence Initialization
            StatsPersistence _statsPersistence = FindFirstObjectByType<StatsPersistence>();
            if (_statsPersistence.isFenrir)
                ActiveCharacter = Character.Fenrir;
            else
                ActiveCharacter = Character.Hel;

            if (_statsPersistence.PlayerHealth > 0)
                _currentHealth = _statsPersistence.PlayerHealth;

            if (_statsPersistence.HealthItemAmount > 0)
                _potionCharges = _statsPersistence.HealthItemAmount;

            #endregion
            SceneManager sceneManager = FindFirstObjectByType<SceneManager>();
            if (sceneManager != null)
                sceneManager.OnSceneLoaded.AddListener(() =>
                {
                    _statsPersistence.PlayerHealth = _currentHealth;
                    _statsPersistence.HealthItemAmount = _potionCharges;
                    _statsPersistence.isFenrir = ActiveCharacter == Character.Fenrir;
                });

            _playerInput.SwitchCurrentActionMap("Dialogue");
            _playerInput.SwitchCurrentActionMap("UI");
            _playerInput.SwitchCurrentActionMap("Player");
           
            _rigidbody.maxAngularVelocity = 0;

            _camera = Camera.main!;

            _inputBuffer = new(_inputBufferMargin);

            _playerBaseStats = (PlayerBaseStats)EntityBaseStats;
            InitializeBaseStats();

            _originalMoveSpeed = _moveSpeed;
            _originalDashDistance = Vector3.Distance(transform.position, _dashPoint.position);

            InitializeAbilityTrackers();
            InitializeAttackPoints();
            InitializeAnimators();

            OnDamageDealt.AddListener((entity, chargeAmount) =>
            {
                if (ActiveCharacter == Character.Fenrir)
                    AddPotionCharges(entity, chargeAmount);
            });

            OnHealthChanged.AddListener((current, max) =>
                FMODEvents.SetLowHealth((float)current / max <= _lowHealthThreshold));

            //Sync the health UI at the start
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            CharacterIndexChanged();
        }

        private void InitializeAbilityTrackers()
        {
            _attackAbilityTrackers = new AttackAbilityTracker[]
            {
                new(_fenrirAbilities.Attack, (ability, action) =>
                    StartAttack(ability, action, ATTACK)),
                new(_helAbilities.Attack, (ability, action) =>
                    StartAttack(ability, action, ATTACK))
            };
            
            _switchAbilityTrackers = new AttackAbilityTracker[]
            {
                new(_fenrirAbilities.Switch, (ability, action) =>
                    StartAttack(ability, action, SWITCH)),
                new(_helAbilities.Switch, (ability, action) =>
                    StartAttack(ability, action, SWITCH))
            };

            _dashAbilityTracker = new(_dashAbility, () => PerformDash(_dashPoint.position, true));
        }

        private void InitializeAttackPoints()
        {
            _attackPoints = new[]
            {
                _fenrirAttackPoint,
                _helAttackPoint
            };
        }

        private void InitializeAnimators()
        {
            _animators = new[]
            {
                _fenrirAnimator,
                _helAnimator
            };
        }

        protected override void InitializeBaseStats()
        {
            base.InitializeBaseStats();

            _critChance = _playerBaseStats.CritChance;
            _critDamage = _playerBaseStats.CritDamage;
        }

        public void LoseControl() => _hasControl = false;
        public void GainControl() => _hasControl = true;

        public void SetDashing(bool isDashing) => _isDashing = isDashing;
        public void SetDashing() => _isDashing = true;

        private void Update()
        {
            _inputBuffer.Update();
            if (_hasControl)
                _inputBuffer.NextInput();

            foreach (var attackAbilityTracker in _attackAbilityTrackers)
                attackAbilityTracker.Update();
            foreach (var switchAbilityTracker in _switchAbilityTrackers)
                switchAbilityTracker.Update();

            _dashAbilityTracker.Update();

            if (!_hasControl && !_isDashing)
            {
                if (_animationLockMoveSpeedFadeDuration == 0)
                {
                    _moveSpeed = 0;
                }
                else
                {
                    float t = Mathf.Clamp01(_controlLossDuration / _animationLockMoveSpeedFadeDuration);
                    _moveSpeed = Mathf.Lerp(_originalMoveSpeed, 0, t);
                }
            }
            else if (!_isDashing)
            {
                _moveSpeed = _originalMoveSpeed;
            }

            MoveAndRotate();

            if (_interactables.Count > 0)
                FindMainInteractable();

            if (!_hasControl)
                _controlLossDuration += Time.deltaTime;
            else
                _controlLossDuration = 0;
        }

        private void MoveAndRotate()
        {
            Vector3 movement = Vector3.zero;
            
            if (!_isDashing && _hasControl)
            {
                var cameraForward = _camera.transform.forward;
                var downProjection = Vector3.Project(cameraForward, Vector3.up);

                var forwardDirection = (cameraForward - downProjection).normalized;
                var rightDirection = _camera.transform.right.normalized;

                Vector3 movementX = _moveInput.x * rightDirection;
                Vector3 movementZ = _moveInput.y * forwardDirection;

                movement = _moveSpeed * (movementX + movementZ).normalized;
            }
            else if (_isDashing)
            {
                movement = _moveSpeed * transform.forward;
            }

            _rigidbody.linearVelocity = movement;
            
            if (_lungeCoroutine != null)
                _rigidbody.linearVelocity += _lungeForce;

            transform.LookAt(transform.position + _rigidbody.linearVelocity);

            if (_knockbackCoroutine != null)
                _rigidbody.linearVelocity += _knockbackForce;

            CurrentAnimator.SetBool(IS_MOVING, movement.magnitude > 0.01f);
        }

        private IEnumerator LungeFadeCoroutine(Vector3 direction, float originalForce, float duration)
        {
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);

                float force = originalForce * Mathf.Abs(Mathf.Pow(t, 3) - 1);
                _lungeForce = force * direction;

                yield return null;
            }

            _lungeForce = Vector3.zero;
            _lungeCoroutine = null;
        }

        private Transform FindTarget()
        {
            var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            
            List<float> distances = new();
            List<float> angles = new();

            var validEnemies = enemies.Where(enemy =>
            {
                if (!enemy.HasSpawned || enemy.IsDead)
                    return false;

                float distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance > _targetLockMaxDistance)
                    return false;

                Vector3 toVector = enemy.transform.position - transform.position;
                float angle = Mathf.Abs(Vector3.Angle(transform.forward, toVector));

                if (angle > _targetLockAngle)
                    return false;

                distances.Add(distance);
                angles.Add(angle);

                return true;
            }).ToArray();

            if (validEnemies.Length == 0)
                return null;

            int targetIndex = 0;
            float maxWeight = 0;
            for (int i = 0; i < validEnemies.Length; i++)
            {
                float distanceWeight = _targetLockDistanceWeight *
                                       (1 - Mathf.Clamp01(distances[i] / _targetLockMaxDistance));
                float angleWeight = _targetLockAngleWeight *
                                    (1 - Mathf.Clamp01(angles[i] / _targetLockAngle));

                float weight = distanceWeight * angleWeight;

                if (weight <= maxWeight)
                    continue;

                maxWeight = weight;
                targetIndex = i;
            }

            return validEnemies[targetIndex].transform;
        }
        
        private void StartAttack(Ability ability, int useTimes, int animatorHash)
        {
            LoseControl();
            var target = FindTarget();
            _targetPos = target ? target.position : transform.position + transform.forward * 10f;
            _targetPos.y = transform.position.y;

            _currentAbility = ability;
            _currentAbilityUseTimes = useTimes;

            CurrentAnimator.SetTrigger(animatorHash);
        }

        public void PerformAttack(Transform attackPoint)
        {
            GainControl();

            if (!_currentAbility)
                return;

            var attackStats = new AttackStats(
                _currentAbility.AttackPrefab,
                _damage,
                _critChance,
                _critDamage);

            transform.LookAt(_targetPos);

            var position = attackPoint.position;

            if (_currentAbility.Burst)
                StartCoroutine(AttackCoroutine(attackStats, _currentAbilityUseTimes,
                    _currentAbility.BurstDelay, _currentAbility.SpreadAngle, position));
            else
                Attack.Create(this, position, transform.rotation, attackStats);
        }

        public void PerformAttackParented(Transform attackPoint)
        {
            GainControl();

            if (!_currentAbility)
                return;

            var attackStats = new AttackStats(
                _currentAbility.AttackPrefab,
                _damage,
                _critChance,
                _critDamage);

            var position = attackPoint.position;

            if (_currentAbility.Burst)
                StartCoroutine(AttackCoroutine(attackStats, _currentAbilityUseTimes,
                    _currentAbility.BurstDelay, _currentAbility.SpreadAngle, position));
            else
                Attack.Create(this, attackPoint, attackStats);
        }

        public void PerformAttackLunge()
        {
            var toTarget = _targetPos - transform.position;
            var distanceToTarget = toTarget.magnitude;
            var direction = toTarget.normalized;

            float projectedDistance = 0.75f * _fenrirLungeForce * _fenrirLungeDuration;
            float duration = _fenrirLungeDuration * Mathf.Clamp01(distanceToTarget / projectedDistance);
            
            if (_lungeCoroutine != null)
            {
                StopCoroutine(_lungeCoroutine);
                _lungeCoroutine = null;
            }

            _lungeCoroutine = StartCoroutine(LungeFadeCoroutine(direction, _fenrirLungeForce, duration));
        }

        private IEnumerator AttackCoroutine(AttackStats stats, int times, float delay, float spreadAngle,
            Vector3 position)
        {
            float halfAngle = spreadAngle * (times - 1) * 0.5f;

            for (int i = 0; i < times; i++)
            {
                float angle = spreadAngle * i - halfAngle;
                Quaternion rotation = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);

                Attack.Create(this, position, rotation, stats);

                if (i != times - 1)
                    yield return new WaitForSeconds(delay);
            }
        }

        private void PerformDash(Vector3 dashPoint, bool animate)
        {
            // Projected dash vector using the calculated offset from player center to front
            Vector3 dashVector = dashPoint - _rigidbody.position;
            float distance = dashVector.magnitude;

            // Distance from center of player to the front collision point
            Vector3 collisionPointOffset =
                dashVector.normalized * (0.02f + _collider.radius * 2);

            LayerMask holeAndWall = _wallLayer | _holeLayer;

            var commands = new NativeArray<RaycastCommand>(1, Allocator.Persistent);
            var hits = new NativeArray<RaycastHit>(3, Allocator.Persistent);
            QueryParameters parameters =
                new QueryParameters(holeAndWall, true, QueryTriggerInteraction.Ignore, true);
            commands[0] = new RaycastCommand(transform.position, dashVector.normalized, parameters,
                distance);
            RaycastCommand.ScheduleBatch(commands, hits, 1, 3).Complete();

            hits.Sort(Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

            //int closestWall = -1;
            int firstHit = -1;
            int hitCount = 0;
            bool hitCollider = false;
            List<Vector3> hitPoints = new List<Vector3>();
            for (int i = 1; i < hits.Length; i++)
            {

                if (hits[i].collider)
                {
                    if (!hitCollider)
                    {
                        firstHit = i;
                        hitCollider = true;
                    }
                    hitPoints.Add(hits[i].point);
                    hitCount++;
                }
                else continue;
                if (1 << hits[i].collider.gameObject.layer == _wallLayer)
                {
                    //closestWall = i;
                    if (i % 2 == 0)
                    {
                        Debug.Log("Wall in hole");
                        dashPoint = hits[i - 1].point - collisionPointOffset;
                        goto coroutine;
                    }
                    dashPoint = hits[i].point - collisionPointOffset;
                    goto coroutine;
                }
            }

            Debug.Log(hitCount);

            /*if (closestWall == 0)
            {
                dashPoint = hits[0].point-collisionPointOffset;
                goto coroutine;
            }*/

            if (firstHit == -1)
            {
                goto coroutine;
            }
            if (hitCount == 3)
            {
                dashPoint = hitPoints[hitCount - 1] - collisionPointOffset;
            }
            else if (hitCount == 2)
            {
                dashPoint = hitPoints[hitCount - 1] + collisionPointOffset;
            }
            else if (hitCount == 1)
            {
                Ray backRay = new(hitPoints[0] + dashVector.normalized * distance, -dashVector.normalized);
                if (!hits[firstHit].collider.Raycast(backRay, out RaycastHit hit, 500))
                {
                    dashPoint = hitPoints[0] - collisionPointOffset;
                    goto coroutine;
                }
                Vector3 holeBack = hit.point;
                float holeDiameter = Vector3.Distance(hitPoints[0], holeBack);

                if (holeDiameter > 300) goto coroutine;
                float dashDistance = Vector3.Distance(hitPoints[0], dashPoint);
                float fraction = dashDistance / holeDiameter;
                Debug.LogWarning(fraction);
                if (fraction > _dashHoleSnapFraction)
                {
                    dashPoint = hit.point + collisionPointOffset;

                }
                else
                {
                    dashPoint = hitPoints[0] - collisionPointOffset;
                    goto coroutine;
                }
            }

            coroutine:
            commands.Dispose();
            hits.Dispose();
            if (Vector3.Distance(transform.position, dashPoint) < .5f)
            {
                Debug.LogWarning("Skipped dash");
                return;
            }

            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
                _moveSpeed = _originalMoveSpeed;
            }
            Debug.DrawLine(dashPoint, dashPoint + Vector3.up * 100, Color.magenta, 10f);
            _dashCoroutine = StartCoroutine(DashCoroutine(dashPoint, animate));
        }

        private IEnumerator DashCoroutine(Vector3 dashPoint, bool animate)
        {
            if (animate)
                CurrentAnimator.SetTrigger(DASH);

            OnDashStarted?.Invoke();
            
            SetDashing(true);
            LoseControl();
            
            int defaultPlayerLayer = gameObject.layer;

            int dashingPlayerLayer = _dashingPlayerLayer;
            int dashLayer = 0;
            while ((dashingPlayerLayer >>= 1) > 0)
                dashLayer++;

            gameObject.layer = dashLayer;

            float actualDashDistance = Vector3.Distance(transform.position, dashPoint);
            float dashDistanceFraction = Mathf.Clamp01(actualDashDistance / _originalDashDistance);

            float dashDuration = _dashDuration * dashDistanceFraction;

            float dashSpeed = actualDashDistance / dashDuration;
            _moveSpeed = dashSpeed;

            float elapsedTime = 0;
            while (elapsedTime < dashDuration)
            {
                Vector3 velocityVector = _rigidbody.linearVelocity * Time.fixedDeltaTime;
                float moveDistance = velocityVector.magnitude;
                float distanceToDashPoint = Vector3.Distance(_rigidbody.position, dashPoint);

                if (moveDistance > distanceToDashPoint)
                {
                    float fraction = distanceToDashPoint / moveDistance;
                    _moveSpeed *= fraction;
                    yield return new WaitForFixedUpdate();
                    break;
                }
                    
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            
            //yield return new WaitForSeconds(dashDuration);

            gameObject.layer = defaultPlayerLayer;

            OnDashFinished?.Invoke();
            
            SetDashing(false);
            GainControl();

            float dashFadeDuration = dashDuration * _dashFade;

            if (dashFadeDuration <= 0)
            {
                _moveSpeed = _originalMoveSpeed;
                yield break;
            }

            elapsedTime = 0;
            while (elapsedTime < dashFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / dashFadeDuration);

                _moveSpeed = Mathf.Lerp(dashSpeed, _originalMoveSpeed, t);

                yield return null;
            }
        }

        public override int TakeDamage(int amount, Entity attacker)
        {
            int reducedDamage = Mathf.CeilToInt(amount * (1 - _damageReduction));
            int realDamage = base.TakeDamage(reducedDamage, attacker);

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            return realDamage;
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void AddPotionCharges(Entity _, int damage)
        {
            if (_potionCharges >= _maxPotionCharges)
                return;

            _potionCharges += damage;
            OnPotionChargesChanged?.Invoke(_potionCharges, _maxPotionCharges);
        }

        private void CharacterIndexChanged()
        {
            for (int i = 0; i < _characterContainer.childCount; i++)
            {
                bool activeState = i == (int)ActiveCharacter;

                var character = _characterContainer.GetChild(i);
                character.gameObject.SetActive(activeState);
            }

            FMODEvents.SetCharacter(ActiveCharacter == Character.Hel);
        }

        #region Collision

        private void FindMainInteractable()
        {
            if (_currentInteractable != null)
                _currentInteractable.Highlighted = false;

            int lowestIndex = 0;
            float highestScore = 0;
            for (int i = 0; i < _interactables.Count; i++)
            {
                var between = (_interactables[i].Position - _rigidbody.position);
                var distance = between.magnitude;
                var direction = between / distance;

                float distScore = 1 - Mathf.Clamp01(distance / 10f);
                var dot = Vector3.Dot(transform.forward, direction);

                float score = dot * _lookWeight + distScore * _distanceWeight;
                if (score > highestScore)
                {
                    lowestIndex = i;
                    highestScore = score;
                }
            }

            _currentInteractable = _interactables[lowestIndex];
            _currentInteractable.Highlighted = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                _interactables.Add(other.GetComponent<IInteractable>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                IInteractable interactable = other.GetComponent<IInteractable>();

                if (interactable == _currentInteractable)
                    _currentInteractable.Highlighted = false;

                _interactables.Remove(interactable);
            }
        }

        #endregion

        #region Input

        public void MoveInput(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void InteractInput(InputAction.CallbackContext context)
        {
            if (!_hasControl || !context.performed)
                return;

            if (_interactables.Count == 0 || _currentInteractable == null)
                return;

            _currentInteractable.Interacted();
        }

        public void AttackInput(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            _inputBuffer.Add(AttackAbilityTracker.TryUse);
        }

        public void HealInput(InputAction.CallbackContext context)
        {
            if (!context.performed || !PotionReady || _currentHealth >= _maxHealth)
                return;

            _inputBuffer.Add(() =>
            {
                Heal(_potionHealAmount);

                _potionCharges -= _potionCost;
                OnPotionChargesChanged?.Invoke(_potionCharges, _maxPotionCharges);

                OnPotionDrunk?.Invoke();

                return true;
            });
        }

        public void DashInput(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            _inputBuffer.Add(_dashAbilityTracker.TryUse);
        }

        public void SwitchInput(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            _inputBuffer.Add(() =>
            {
                ActiveCharacter = (Character)((int)++ActiveCharacter % 2);
                CharacterIndexChanged();

                if (!SwitchAbilityTracker.TryUse())
                {
                    ActiveCharacter = (Character)((int)++ActiveCharacter % 2);
                    CharacterIndexChanged();
                    return false;
                }

                foreach (var tracker in _switchAbilityTrackers)
                    tracker.Reset();

                return true;
            });
        }

        #endregion

        protected override void Die()
        {
            base.Die();
            _statsPersistence.PlayerHealth = _maxHealth;
            _statsPersistence.HealthItemAmount = 0;
        }
    }
}