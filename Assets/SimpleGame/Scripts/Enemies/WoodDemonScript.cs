using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts.Enemies
{
    public class WoodDemonScript : MonoBehaviour
    {
        public float PlayerDist = 1;
        public float AttackInterval = 1;
        public float MoveSpeed = 1;
        public float distToGround = 1;
        public float JumpSpeed = 5;
        public float randJump = 0.1f;
        public float SphereCastRadius = 0.3f;
        public float notMovingThreshold = 0.5f;

        private PlayerScript player;
        private Rigidbody rigidbody;

        public void Start()
        {
            player = PlayerScript.Instance;
            rigidbody = GetComponent<Rigidbody>();
        }

        private Vector3 desiredPos;

        private float timeStuck = 0;

        private Vector3 lastPos;


        private float lastAttack = float.MinValue;

        public void FixedUpdate()
        {
            if ((player.GetPlayerPosition() - transform.position).magnitude < PlayerDist)
            {
                // Attack!
                if (lastAttack + AttackInterval < Time.timeSinceLevelLoad)
                {
                    lastAttack = Time.timeSinceLevelLoad;
                    player.TakeDamage(10);
                }
                return;

            }

            //Debug.Log(canJump() + "    " + isStuck());
            if (isStuck()) timeStuck += Time.fixedDeltaTime;
            else timeStuck = 0;
                    
            if (canJump() && timeStuck > 0.2f)
            {
                rigidbody.velocity = rigidbody.velocity.ChangeY(JumpSpeed);
            }

            var dir = (player.GetPlayerPosition() - transform.position).normalized;

            rigidbody.velocity = (dir * MoveSpeed).ChangeY(rigidbody.velocity.y);

            lastPos = rigidbody.position;
        }

        private bool isStuck()
        {
            if ((player.GetPlayerPosition() - transform.position).magnitude < 0.1f) return false;
            if ((lastPos - rigidbody.position).magnitude > Time.fixedDeltaTime * MoveSpeed * notMovingThreshold) return false;
            return true;
        }

        private bool canJump()
        {
            return IsGrounded() && rigidbody.velocity.y < 0.01f;
        }

        bool IsGrounded()
        {
            var ray = new Ray(transform.position, -Vector3.up);
            return Physics.SphereCast(ray, SphereCastRadius, distToGround + 0.1f - SphereCastRadius);
        }
    }
}