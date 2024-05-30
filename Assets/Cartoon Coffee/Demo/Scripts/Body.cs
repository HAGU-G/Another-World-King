using UnityEngine;

namespace Demo_Project
{
    public class Body : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SceneManager.listOfBodies.Add(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}