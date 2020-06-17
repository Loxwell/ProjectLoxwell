using UnityEngine;

namespace Platformer.Core
{
    /// <summary>
    /// Fuzzy provides methods for using values +- an amount of random deviation(편차), or fuzz.
    /// 비정상적인 데이터를 어플리케이션에 전달하여 에러를 유도하는 테스트 
    /// - 소프트웨어에 무작위의 데이터를 반복적으로 입력하여 조직적인 실패를 유발함으로써 보안상의 취약점을 찾아내는 테스트
    /// - Blackbox, Penertation(침투), Robustness
    /// </summary>
    public static class FuzzTesting
    {
        public static bool ValueLessThan(float value, float test, float fuzz = 0.1f)
        {
            float delta = value - test;
            return delta < 0 ? true : UnityEngine.Random.value > delta / (fuzz * test);
        }

        public static bool ValueGreaterThan(float value, float test, float fuzz = 0.1f)
        {
            float delta = value - test;
            return delta < 0 ? UnityEngine.Random.value > -1 * delta / (fuzz * test) : true;
        }

        public static bool ValueNear(float value , float test, float fuzz = 0.1f)
        {
            return Mathf.Abs(1f - (value / test)) < fuzz;
        }

        public static float Value(float value, float fuzz = 0.1f)
        {
            return value + value * UnityEngine.Random.Range(-fuzz, fuzz);
        }
    }
}

