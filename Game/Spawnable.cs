using System.Collections.Generic;
using UnityEngine;
using ShootAR.Enemies;

namespace ShootAR
{
    /// <summary>
    /// 생성 가능한(스폰 가능한) 오브젝트의 기본 클래스입니다.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public abstract class Spawnable : MonoBehaviour
    {
        /// <summary>
        /// 전역 스폰 제한 수입니다.
        /// </summary>
        public const int GLOBAL_SPAWN_LIMIT = 30;

        [SerializeField, Range(0f, Mathf.Infinity)] private float speed;
        /// <summary>
        /// 오브젝트의 이동 속도입니다.
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set
            {
                if (value < 0)
                    throw new UnityException("Speed can not be a negative number.");

                speed = value;
            }
        }

        /// <summary>
        /// 이미 인스턴스화된 오브젝트가 요청될 때 사용할 수 있는 오브젝트 풀을 포함하는 클래스입니다.
        /// </summary>
        /// <typeparam name="T">
        /// 풀에 매칭되는 오브젝트의 유형
        /// </typeparam>
        public class Pool<T> where T : Spawnable
        {
            private static Pool<T> instance;

            /// <summary>
            /// 풀의 인스턴스입니다.
            /// </summary>
            public static Pool<T> Instance
            {
                get
                {
                    if (instance == null) instance = new Pool<T>();
                    else if (instance.objectStack.Count > 0 && instance.objectStack.Peek() == null)
                        instance.Empty();

                    return instance;
                }
            }

            internal Stack<T> objectStack = new Stack<T>(GLOBAL_SPAWN_LIMIT);

            private Pool() { }

            /// <summary>
            /// 풀에 있는 오브젝트의 수입니다.
            /// </summary>
            public int Count { get => objectStack.Count; }

            /// <summary>
            /// 풀을 <paramref name="referenceObject"/>의 복사본으로 채웁니다.
            /// </summary>
            /// <param name="referenceObject">풀을 채울 기준 오브젝트</param>
            /// <param name="lot">풀에 추가할 오브젝트 수</param>
            public void Populate(T referenceObject, int lot = GLOBAL_SPAWN_LIMIT)
            {
                if (objectStack.Count > 0)
                    throw new UnityException("Trying to populate an already populated pool.");
                else
                    for (int i = 0; i < lot; i++)
                    {
                        T spawnedObject = Instantiate(referenceObject);
                        spawnedObject.gameObject.SetActive(false);
                        objectStack.Push(spawnedObject);
                    }
            }

            /// <summary>
            /// 리소스에서 로드된 오브젝트의 복사본으로 풀을 채웁니다.
            /// 어떤 파일이 로드되는지는 풀의 유형에 따라 결정됩니다.
            /// </summary>
            /// <param name="lot">풀에 추가할 오브젝트 수</param>
            public void Populate(int lot = GLOBAL_SPAWN_LIMIT)
            {
                string prefab = "";
                if (typeof(T) == typeof(Crasher))
                    prefab = Prefabs.CRASHER;
                else if (typeof(T) == typeof(Drone))
                    prefab = Prefabs.DRONE;
                else if (typeof(T) == typeof(ArmorCapsule))
                    prefab = Prefabs.ARMOR_CAPSULE;
                else if (typeof(T) == typeof(HealthCapsule))
                    prefab = Prefabs.HEALTH_CAPSULE;
                else if (typeof(T) == typeof(PowerUpCapsule))
                    prefab = Prefabs.POWER_UP_CAPSULE;
                else if (typeof(T) == typeof(BulletCapsule))
                    prefab = Prefabs.BULLET_CAPSULE;
                else if (typeof(T) == typeof(EnemyBullet))
                    prefab = Prefabs.ENEMY_BULLET;
                else if (typeof(T) == typeof(Bullet))
                    prefab = Prefabs.BULLET;
                else if (typeof(T) == typeof(Portal))
                    prefab = Prefabs.PORTAL;

                Populate(Resources.Load<T>(prefab), lot);
            }

            /// <summary>
            /// <typeparamref name="T"/> 유형의 오브젝트를 적절한 풀에서 요청합니다.
            /// </summary>
            /// <returns>
            /// <typeparamref name="T"/> 유형의 사용 가능한 오브젝트에 대한 참조
            /// </returns>
            public T RequestObject()
            {
                if (objectStack.Count == 0) return null;

                return objectStack.Pop();
            }

            /// <summary>
            /// 풀에 포함된 모든 오브젝트를 참조 해제합니다.
            /// </summary>
            public void Empty()
            {
                objectStack.Clear();
            }
        }

        /// <summary>
        /// 오브젝트를 비활성화하고 다시 사용될 수 있도록 풀에 반환합니다.
        /// </summary>
        /// <typeparam name="T">
        /// 오브젝트가 반환될 풀의 유형
        /// </typeparam>
        public void ReturnToPool<T>() where T : Spawnable
        {
            gameObject.SetActive(false);
            Pool<T>.Instance.objectStack.Push((T)this);
            ResetState();
        }

        /// <summary>
        /// 오브젝트를 기본값으로 재설정합니다.
        /// </summary>
        public abstract void ResetState();

        /// <summary>
        /// 오브젝트를 파괴합니다.
        /// </summary>
        public abstract void Destroy();
    }
}
