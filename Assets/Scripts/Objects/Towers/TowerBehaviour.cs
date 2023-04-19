using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Towers
{
    public class TowerBehaviour : MonoBehaviour
    {
        [SerializeReference] private ProjectileBasic projectilePrefab;

        private static Dictionary<ShipBehavior,TargetingInfo> _targetingInfoDict;
        private static ShipListSO _shipList; 
        private void Awake()
        {
            if (_shipList == null)
            {
                _shipList = FindObjectOfType<ShipListSO>();
            }
        }

        [SerializeField] private float firingCooldown = 2.0f;
        private float _firingTimer = Random.Range(0.0f, 2.0f);
        [SerializeField] private float precisionTolerance = 3.0f;//probably fine for it to be fairly large
        [SerializeField] private int maxTargetingIteration = 150;//probably a bit overkill

        private void Update()
        {
            _firingTimer += Time.deltaTime;
            if (!(_firingTimer > firingCooldown)) return;//cooldown
            if (!ComputeTarget(_shipList.ShipList, out var target)) return;//no valid target
            Shoot(target);
        }

        private bool ComputeDestination(ITargetable target,ref Vector2 destination)
        {
            //the old ballistic bs
            //this seems like a brute force method
            //TODO: find parametrisation of the minimum distance and find positive root of that
            target.getTargetingInfo(out var targetOrigin,out var targetSpeed, out var targetDestination);
            var towerPosition = transform.position;
            var deltaOtOs = towerPosition - targetOrigin;
            var deltaOtDs = towerPosition - targetDestination;
            var angleOrigin = Vector2.Angle(Vector2.right, deltaOtOs);
            var angleDestination = Vector2.Angle(Vector2.right, deltaOtDs);
            var interpolationFactor = 1f;//we check beyond the destination first in case we just need to aim at the destination
            var interpolationStep = 1f;
            var interationCounter = 0;
            var progressDir = 1;
            for (int iterationCounter = 0; iterationCounter < maxTargetingIteration; iterationCounter++)
            {
                if (interpolationFactor > 1.0f)
                {
                    destination = targetDestination;
                    return true;
                }
                //Vector2 slerp but lerp could be good enough
                var candidateDirection = Vector2.Lerp(deltaOtOs, deltaOtDs, interpolationFactor).normalized;
                //todo what's the minimum distance between the projectile and target 

            }
            throw new NotImplementedException();
        }

        private bool ComputeTarget(IReadOnlyCollection<ShipBehavior> shipList , out ShipBehavior firingTarget)//could also be a collection of ITargets
        {
            if (shipList.Count == 0)
            {
                firingTarget = null;
                return false;
            }
            ShipBehavior target = null;
            var targetDist = Vector3.positiveInfinity;
            var position = transform.position;
            var targetAcquired = false;
            foreach (var ship in shipList)
            {
                //insert other validity check here
                if (_targetingInfoDict.TryGetValue(ship,out var targetInfo) && targetInfo.EffectiveRemainingHitPoint <= 0) continue;//discard ship that will be destroyed by already fired ships
                var shipDist = ship.transform.position - position;
                if (shipDist.magnitude < targetDist.magnitude)
                {
                    targetDist = shipDist;
                    target = ship;
                    targetAcquired = true;
                }
            }
            firingTarget = target;
            return targetAcquired;
        }


        void Shoot(ITargetable target)
        {
            _firingTimer = 0f;
            Vector2 destination = default;
            if (!ComputeDestination(target, ref destination)) return;
            Instantiate(projectilePrefab).SetDestination(destination);
            //TODO: update targeting info
        }

        private struct TargetingInfo
        {
            public int EffectiveRemainingHitPoint;
        }
    }
    
    
}
