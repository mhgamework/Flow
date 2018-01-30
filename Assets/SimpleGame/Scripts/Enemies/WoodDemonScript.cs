﻿using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts.Enemies
{
    public class WoodDemonScript : MonoBehaviour
    {
        public float PlayerDist = 1;
        public float AttackInterval = 1;
        public float PlayerDetectionDistance = 20;

        public float RandomWalkChance;
        public float RandomWalkDistance;
        private bool isRandomWalking = false;
        private Vector3 randomWalkTarget;

        public float MoveSpeed = 1;
        public float distToGround = 1;
        public float JumpSpeed = 5;
        public float randJump = 0.1f;
        public float SphereCastRadius = 0.3f;
        public float notMovingThreshold = 0.5f;


        public bool IsDigger = false;
        public float DigHoleRadius = 3;
        public float DigMovementSpeed = 0.5f;
        public VoxelRenderingEngineScript Renderer;
        public bool isDigging = false;


        private PlayerScript player;
        private Rigidbody rigidbody;

        public Transform Model;
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
          
            if (DayNightCycleScript.Instance.IsDay())
            {
                Hide();
                return;
            }


            Show();

            if (!canDetectPlayer())
            {
                TryRandomWalk();
                return;

            }

            //if (IsDigger && isWayBlocked(player.GetPlayerPosition()))
            //{
            //    Debug.Log("Blocked");
            //    dig(player.GetPlayerPosition());
            //    return;
            //}
            if (IsDigger)
            {
                dig(player.GetPlayerPosition());
            }
            if (CanAttackPlayer())
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
            MoveTo(player.GetPlayerPosition());
        }

        private void dig(Vector3 target)
        {
            if (Renderer.GetWorld() == null) return;
            var cil = new Cylinder(DigHoleRadius, 2);

            var cilForward = new Transformation(cil,
                Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.up, Vector3.forward))
                * Matrix4x4.Translate(new Vector3(0, 2, 0)));

            var floor = new Translation(new Box(8, DigHoleRadius, 8), new Vector3(0, -DigHoleRadius, 0));

            /*var transform = new Transformation(cil,( 
                Matrix4x4.Translate(new Vector3(0, 2, 0)) *
                Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.up, (target - this.transform.position).normalized)) *
                Matrix4x4.Translate(this.transform.position)).inverse);*/

            var transform = new Transformation(new Subtraction(floor, cilForward),
                Matrix4x4.Translate(this.transform.position - Vector3.up)
                * Matrix4x4.Rotate(Quaternion.LookRotation((target - this.transform.position).normalized, Vector3.up)));

            transform = new Transformation(new Subtraction(cilForward, floor),

                Matrix4x4.Translate(this.transform.position - Vector3.up)
                * Matrix4x4.Rotate(Quaternion.LookRotation((target - this.transform.position).normalized, Vector3.up))
                );

            bool hasDug = false;
            Renderer.GetWorld().RunKernel1by1((this.transform.position - Vector3.one * 3).ToFloored(),
                (this.transform.position + Vector3.one * 3).ToCeiled(),
                (d, p) =>
                {
                    var newDensity = Mathf.Max(d.Density, -transform.Sdf(p));

                    hasDug = hasDug || (newDensity - d.Density > 0.01);


                    d.Density = newDensity;



                    return d;
                }, Time.frameCount);

            Debug.Log(isDigging);
            isDigging = hasDug;
        }

        private bool isWayBlocked(Vector3 target)
        {
            var dir = (transform.position - target).normalized;
            return Physics.CapsuleCast(new Vector3(0, 0.5f, 0), new Vector3(0, 1.5f, 0), 1, dir, 2);
        }

        private bool CanAttackPlayer()
        {
            return (player.GetPlayerPosition() - transform.position).magnitude < PlayerDist;
        }

        private bool canDetectPlayer()
        {
            return ((player.GetPlayerPosition() - transform.position).magnitude < PlayerDetectionDistance);
        }

        private void MoveTo(Vector3 targetPos)
        {
            if (isStuck(targetPos)) timeStuck += Time.fixedDeltaTime;
            else timeStuck = 0;

            if (canJump() && timeStuck > 0.2f)
            {
                rigidbody.velocity = rigidbody.velocity.ChangeY(JumpSpeed);
            }

            var dir = (targetPos - transform.position).normalized;

            var speed = getActualMovespeed();

            rigidbody.velocity = (dir * speed).ChangeY(rigidbody.velocity.y);

            lastPos = rigidbody.position;
        }

        private float getActualMovespeed()
        {
            return isDigging && IsDigger ? DigMovementSpeed : MoveSpeed;
        }

        private void TryRandomWalk()
        {
            randomWalkTarget.y = transform.position.y;
            if (isRandomWalking)
            {
                MoveTo(randomWalkTarget);
                if (Vector3.Distance(randomWalkTarget, transform.position) < 1)
                {
                    isRandomWalking = false;
                    rigidbody.velocity = new Vector3(0, 0, 0);
                }
            }

            if (Random.value < RandomWalkChance)
            {
                var dist = Random.value * RandomWalkDistance;

                var dir = Random.insideUnitCircle.ToXZ(0);
                isRandomWalking = true;
                randomWalkTarget = transform.position + dir * dist;
            }
        }

        private void Show()
        {
            Model.gameObject.SetActive(true);
            rigidbody.isKinematic = false;
        }

        private void Hide()
        {
            Model.gameObject.SetActive(false);
            rigidbody.isKinematic = true;

        }

        private bool isStuck(Vector3 targetPos)
        {
            if ((targetPos - transform.position).magnitude < 0.1f / 3f * getActualMovespeed()) return false;
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