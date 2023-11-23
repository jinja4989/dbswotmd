using UnityEngine;

namespace ShootAR {
    /// <summary>
    /// 포탈 오브젝트를 나타내는 클래스입니다.
    /// </summary>
    public class Portal : Spawnable {
        [SerializeField] // Inspector에서 할당합니다.
        private new ParticleSystem animation; // Unity에는 이미 해당 이름으로 사용된 필드가 있어서 new를 사용합니다.

        /// <summary>
        /// 오브젝트가 활성화될 때 호출되는 메서드입니다.
        /// </summary>
        public void OnEnable() {
            animation.Play();
        }

        /// <summary>
        /// 오브젝트 상태를 초기화하는 메서드입니다.
        /// </summary>
        public override void ResetState() {
            animation.Stop();
        }

        /// <summary>
        /// 오브젝트를 파괴하는 메서드입니다.
        /// </summary>
        public override void Destroy() {
            ReturnToPool<Portal>();
        }
    }
}

