using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts.Enemies
{
    /// <summary>
    /// Moving behaviour for moving a capsule rigibody
    /// Includes walking and jumping over obstacles
    /// Fairly stupid but does at least something
    /// </summary>
    public class AiCapsuleMovingBehaviour
    {
        private Rigidbody rigidbody;
        private Transform transform;


        public float JumpSpeed = 5;
        public float SphereCastRadius = 0.3f;
        public float NotMovingThreshold = 0.5f;
        public float MoveSpeed = 1;
        public float GoalDistToGround = 1;

        private float timeStuck = 0;
        private Vector3 lastPos;
        




        public AiCapsuleMovingBehaviour(Rigidbody rigidbody, Transform transform)
        {
            this.rigidbody = rigidbody;
            this.transform = transform;
        }

        public void MoveTo(Vector3 targetPos)
        {
            if (isStuck(targetPos)) timeStuck += Time.fixedDeltaTime;
            else timeStuck = 0;

            if (canJump() && timeStuck > 0.2f)
            {
                rigidbody.velocity = rigidbody.velocity.ChangeY(JumpSpeed);
            }

            var dir = (targetPos - transform.position).normalized;


            rigidbody.velocity = (dir * MoveSpeed).ChangeY(rigidbody.velocity.y);

            lastPos = rigidbody.position;
        }

        private bool isStuck(Vector3 targetPos)
        {
            if ((targetPos - transform.position).magnitude < 0.1f / 3f * MoveSpeed) return false;
            if ((lastPos - rigidbody.position).magnitude > Time.fixedDeltaTime * MoveSpeed * NotMovingThreshold) return false;
            return true;
        }


        private bool canJump()
        {
            return IsGrounded() && rigidbody.velocity.y < 0.01f;
        }

        bool IsGrounded()
        {
            var ray = new Ray(transform.position, -Vector3.up);
            return Physics.SphereCast(ray, SphereCastRadius, GoalDistToGround + 0.1f - SphereCastRadius);
        }
    }
}