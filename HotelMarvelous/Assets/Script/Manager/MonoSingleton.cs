using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Singleton          //팀작업시 같은이름의 싱글톤중복을 피하기위해 네임스페이스로 나눈다.
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour           //T는 타입값 인트,플로트 뭐든지 들어갈 수 있다
    {
        protected static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)       //최초에만 생성하기위해
                {
                    //생성
                    instance = new GameObject(typeof(T).ToString()).AddComponent<T>();       //타입에 들어간 이름으로 생성해 안에 컴포넌트를 넣어 생성 씬재생시 하이라키의 DontDestroyOnLoad확인
                    DontDestroyOnLoad(Instance);        //씬로드시에도 유지
                }

                return instance;        //최초생성되어있을경우 리턴
            }
        }
    }

    public class Singleton<T> where T : class
    {
        protected static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    //생성
                }

                instance = System.Activator.CreateInstance(typeof(T)) as T;

                return instance;
            }
        }

    }
}

