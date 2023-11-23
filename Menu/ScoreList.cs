using System.IO;
using UnityEngine;

namespace ShootAR
{
    /// <summary>
    /// 고스코어 목록을 나타내며, 플레이어 이름과 점수를 관리합니다.
    /// </summary>
    public class ScoreList
    {
        /// <summary>
        /// 목록의 최대 위치 수입니다.
        /// </summary>
        public const int POSITIONS = 10;

        private string[] name = new string[POSITIONS];
        private ulong[] score = new ulong[POSITIONS];

        /// <summary>
        /// 목록의 적절한 위치에 점수를 추가합니다.
        /// </summary>
        /// <param name="name">점수를 달성한 플레이어의 이름입니다.</param>
        /// <param name="score">목록에 추가할 점수입니다.</param>
        /// <returns>
        /// 목록에 추가되면 true를 반환합니다.
        /// <paramref name="score"/>가 기존 점수보다 낮으면 false를 반환합니다.
        /// </returns>
        public bool AddScore(string name, ulong score)
        {
            // 모든 기존 점수보다 낮은 경우 즉시 반환합니다.
            if (score < this.score[POSITIONS - 1]) return false;

            for (int i = 0; i < POSITIONS; i++)
            {
                // 점수가 들어갈 위치를 찾습니다.
                if (score > this.score[i])
                {
                    // 낮은 점수를 한 자리 옮깁니다 (가장 낮은 점수를 삭제합니다).
                    for (int j = POSITIONS - 1; j > i; j--)
                    {
                        this.score[j] = this.score[j - 1];
                        this.name[j] = this.name[j - 1];
                    }

                    // 해당 위치를 새로운 점수로 교체합니다.
                    this.score[i] = score;
                    this.name[i] = name;

                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// 목록에서 <paramref name="position"/> 위치에 있는 점수의 이름과 점수를 반환합니다.
        /// </summary>
        /// <param name="position">점수의 위치입니다.</param>
        /// <returns>점수 정보를 포함하는 튜플입니다.</returns>
        public (string, ulong) this[int position] => (name[position], score[position]);

        /// <summary>
        /// 파일에서 고스코어 테이블을 로드합니다.
        /// </summary>
        /// <returns>로드된 점수를 포함하는 <see cref="ScoreList"/>입니다.</returns>
        public static ScoreList LoadScores()
        {
            ScoreList scores = new ScoreList();

            using (BinaryReader reader = new BinaryReader(
                new FileInfo(Configuration.Instance.Highscores.FullName).OpenRead()))
            {
                for (int i = 0; i < 10; i++)
                {
                    scores.AddScore(
                        reader.ReadString(),
                        reader.ReadUInt64()
                    );
                }
            }

            return scores;
        }

        /// <summary>
        /// 특정 점수가 존재하는지 확인합니다.
        /// </summary>
        /// <param name="score">확인할 점수입니다.</param>
        /// <returns>존재하면 true를 반환합니다.</returns>
        public bool Exists(ulong score)
        {
            bool answer = false;

            foreach (ulong s in this.score)
            {
                if (s == score)
                {
                    answer = true;
                    break;
                }
            }

            return answer;
        }
    }
}
