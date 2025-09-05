using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProfilerReference.Demo
{
    /// <summary>
    /// Script demo để test Profiler Reference Tool
    /// Chứa các ví dụ về code có thể gây performance issue
    /// </summary>
    public class ExamplePerformanceScript : MonoBehaviour
    {
        [Header("Demo Settings")]
        public int objectCount = 1000;
        public bool enableInefficientCode = true;

        private List<GameObject> _objects = new List<GameObject>();
        private string _repeatedString;

        private void Start()
        {
            // Tạo nhiều object để test instantiation
            for (int i = 0; i < objectCount; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = Random.insideUnitSphere * 10;
                _objects.Add(obj);
            }

            Debug.Log($"Created {objectCount} objects for performance testing");
        }

        private void Update()
        {
            if (!enableInefficientCode) return;

            // Ví dụ 1: FindObjectsOfType mỗi frame (INEFFICIENT)
            var cameras = FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                // Do something with camera
                var distance = Vector3.Distance(transform.position, cam.transform.position);
            }

            // Ví dụ 2: String concatenation trong loop (INEFFICIENT)
            _repeatedString = "";
            for (int i = 0; i < 100; i++)
            {
                _repeatedString += "test" + i.ToString();
            }

            // Ví dụ 3: Instantiate/Destroy thường xuyên (INEFFICIENT)
            if (Random.value < 0.01f) // 1% chance mỗi frame
            {
                var tempObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(tempObj, 0.1f);
            }

            // Ví dụ 4: Complex calculations mỗi frame
            float result = 0f;
            for (int i = 0; i < 1000; i++)
            {
                result += Mathf.Sin(i) * Mathf.Cos(i);
            }

            // Ví dụ 5: GetComponent mỗi frame (INEFFICIENT)
            foreach (var obj in _objects)
            {
                if (obj != null)
                {
                    var renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.Lerp(Color.red, Color.blue, Mathf.PingPong(Time.time, 1));
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (!enableInefficientCode) return;

            // Ví dụ: Physics calculations phức tạp
            var colliders = Physics.OverlapSphere(transform.position, 5f);
            foreach (var collider in colliders)
            {
                var rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * 0.1f);
                }
            }
        }

        private void OnGUI()
        {
            if (!enableInefficientCode) return;

            // Ví dụ: OnGUI code (có thể inefficient nếu phức tạp)
            GUI.Label(new Rect(10, 10, 300, 20), $"Performance Demo Active - Objects: {_objects.Count}");
            GUI.Label(new Rect(10, 30, 300, 20), $"String Length: {_repeatedString.Length}");

            for (int i = 0; i < 10; i++)
            {
                GUI.Label(new Rect(10, 50 + i * 20, 200, 20), $"Label {i}: {Random.value}");
            }
        }

        /// <summary>
        /// Method được gọi thường xuyên để test profiler
        /// </summary>
        public void ExpensiveMethod()
        {
            // Simulate expensive computation
            var list = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(i * i);
            }

            // Memory allocation
            var array = new byte[1024 * 100]; // 100KB

            Debug.Log($"Expensive method completed. List count: {list.Count}");
        }

        private void OnDestroy()
        {
            // Cleanup
            foreach (var obj in _objects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            _objects.Clear();
        }
    }
}
