using UnityEngine;

public class helper : MonoBehaviour
{
    public static bool JumpButtonHolding()
    {

        if (Input.GetKey(KeyCode.Space)
             || Input.GetKey(KeyCode.A)
             || Input.GetKey(KeyCode.S)
             || Input.GetKey(KeyCode.D)
             || Input.GetKey(KeyCode.F)
             || Input.GetKey(KeyCode.G)
             || Input.GetKey(KeyCode.H)
             || Input.GetKey(KeyCode.J)
             || Input.GetKey(KeyCode.K)
             || Input.GetKey(KeyCode.L)
             || Input.GetButton("Fire1"))
            return true;
        else
            return false;
    }

    public static bool JumpButtonDown()
    {
        if (Input.GetKeyDown(KeyCode.Space)
                    || Input.GetKeyDown(KeyCode.A)
                    || Input.GetKeyDown(KeyCode.S)
                    || Input.GetKeyDown(KeyCode.D)
                    || Input.GetKeyDown(KeyCode.F)
                    || Input.GetKeyDown(KeyCode.G)
                    || Input.GetKeyDown(KeyCode.H)
                    || Input.GetKeyDown(KeyCode.J)
                    || Input.GetKeyDown(KeyCode.K)
                    || Input.GetKeyDown(KeyCode.L)
                    || Input.GetButtonDown("Fire1"))
            return true;
        else
            return false;
    }
}


//using UnityEngine;

//public class helper : MonoBehaviour
//{
//    public static bool JumpButtonHolding()
//    {
//#if UNITY_STANDALONE || UNITY_STANDALONE_WIN || UNITY_WSA

//        if (Input.GetKey(KeyCode.Space)
//             || Input.GetKey(KeyCode.A)
//             || Input.GetKey(KeyCode.S)
//             || Input.GetKey(KeyCode.D)
//             || Input.GetKey(KeyCode.F)
//             || Input.GetKey(KeyCode.G)
//             || Input.GetKey(KeyCode.H)
//             || Input.GetKey(KeyCode.J)
//             || Input.GetKey(KeyCode.K)
//             || Input.GetKey(KeyCode.L)
//             || Input.GetButton("Fire1"))
//            return true;
//        else
//            return false;

//#endif

//#if UNITY_ANDROID || UNITY_IOS

//        if (Input.GetButton("Fire1"))
//            return true;
//        else
//            return false;

//#endif
//    }

//    public static bool JumpButtonDown()
//    {
//#if UNITY_ANDROID || UNITY_IOS

//        if (Input.GetButtonDown("Fire1"))
//            return true;
//        else
//            return false;

//#endif
//    }
//}

