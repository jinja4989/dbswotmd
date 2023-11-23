using UnityEngine;

namespace ShootAR.Enemies
{
	/// <summary>
	/// 단거리 적 클래스
	/// </summary>
	public abstract class Boopboop : Enemy
	{
		/// <summary>
		/// 플레이어에게 피해를 입힙니다.
		///
		/// 플레이어에 대한 성공적인 공격이 발생하면 트리거됩니다.
		/// </summary>
		/// <param name="target">플레이어 객체</param>
		protected abstract void Harm(Player target);

		/// <summary>
		/// 다른 객체와의 충돌을 감지합니다.
		///
		/// 플레이어와 충돌하면 플레이어에게 피해를 입힙니다.
		/// </summary>
		/// <param name="other">충돌한 다른 객체</param>
		protected virtual void OnTriggerEnter(Collider other) {
			var target = other.GetComponent<Player>();
			if (target != null) Harm(target);
		}
	}
}

