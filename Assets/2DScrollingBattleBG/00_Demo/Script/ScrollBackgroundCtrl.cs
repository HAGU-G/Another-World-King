using UnityEngine;

namespace ScrollBGTest
{
    [System.Serializable]
    /// <summary>
    //This script is used for background scrolling demo play.
    //Used in the editor and android.
    //This is a sample script and stability and optimization are not guaranteed.
    /// </summary>

    public class ScrollBackgroundCtrl : MonoBehaviour
    {
        //Background Layers
        public Transform[] Background;

        //Scrolling Speeds
        public float[] ScrollSpeed;

        //Renderer
        public MeshRenderer[] Ren;
        public MeshRenderer SkyRen;

        //Movement speed according to keyboard input
        public float MoveValue;
        public float MoveSpeed;

        //Scroll of the sky
        float SkyMoveValue;
        public float SkyScrollSpeed;

        public AudioClip bgm;

        void Start()
        {
            //Get MeshRenderers
            for (int i = 0; i < Background.Length; i++)
                Ren[i] = Background[i].GetComponent<MeshRenderer>();

            if (bgm != null)
                GameManager.PlayMusic(bgm);
        }


        void Update()
        {
            //Material OffSet
            for (int i = 0; i < Background.Length; i++)
                Ren[i].material.mainTextureOffset = new Vector2(MoveValue * ScrollSpeed[i], 0);

            SkyRen.material.mainTextureOffset = new Vector2(SkyMoveValue += (Time.unscaledDeltaTime * -SkyScrollSpeed), 0);
        }

    }



}
