using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PenguinRun
{
    public class CharacterController : MonoBehaviour
    {

        private Animator m_Animator;
        private ParticleSystem m_DustParticleSystem;
        private Rigidbody2D m_Rigidbody;

        private const float EASY_WALK_SPEED = 1f;
        private const float MEDIUM_WALK_SPEED = 1.5f;
        private const float HARD_WALK_SPEED = 2f;


        private void Awake()
        {
            m_Animator = gameObject.transform.GetComponent<Animator>();
            m_DustParticleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
            m_Rigidbody = gameObject.GetComponent<Rigidbody2D>();
            InitialiseAnimator();
        }

        public void Jump(bool state)
        {
            m_Animator.SetBool("isJumping", state);
            //PlayDustAnimation();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //if (collision.gameObject.tag == "Hazard")
                //GameController.Instance.playerState = PlayerState.Dead;
            if (collision.gameObject.tag == "Path")
                Debug.Log($"{collision.gameObject.name}Tag");
        }

        private void PlayDustAnimation()
        {
            m_DustParticleSystem.Play();
        }

        public void SetWalkSpeed(GameDifficulty currentDifficulty)
        {
            switch (currentDifficulty)
            {
                case GameDifficulty.Easy:
                    m_Animator.speed = EASY_WALK_SPEED;
                    break;
                case GameDifficulty.Medium:
                    m_Animator.speed = MEDIUM_WALK_SPEED;
                    break;
                case GameDifficulty.Hard:
                    m_Animator.speed = HARD_WALK_SPEED;
                    break;
            }
        }

        private void InitialiseAnimator()
        {
            //AnimationEvent jumpAnimation = m_Animator.runtimeAnimatorController.animationClips[1];
            //jumpAnimation.functionName = "PlayDustAnimation";
            //Debug.LogError($"{jumpAnimation}");
            //jumpAnimation.
        }

        //Approfondire
        public void OnAnimationEvent(AnimationEvent evt)
        {
            PlayDustAnimation();
        }
    }
}
