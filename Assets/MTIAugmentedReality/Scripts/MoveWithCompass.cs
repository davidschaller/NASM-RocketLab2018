using UnityEngine;

public class MoveWithCompass : MonoBehaviour
{
	public float pitchOffsetPositiveUp = 15;
    private double _lastCompassUpdateTime = 0;
    private Quaternion _correction = Quaternion.identity;
    private Quaternion _targetCorrection = Quaternion.identity;
    private Quaternion _compassOrientation = Quaternion.identity;

	public static MoveWithCompass main;
	public enum OrientationSensor
	{
		None,
		Gyro,
		Accel
	}
	OrientationSensor sensor;
	public static bool invertCamera = false;
	public static Transform ghost;
    void Start()
    {
		main = this;
		
		GameObject g = new GameObject("Ghost");
		ghost = g.transform;
		
		ghost.transform.parent = transform.parent;
		g.AddComponent<Rigidbody>();
		g.GetComponent<Rigidbody>().isKinematic = true;
		g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		
		if (PersistentData.Main.virtualWalkthrough)
		{
			Debug.LogWarning("VIRTUAL controls...");
			
			virtualTour = true;
			sensor = OrientationSensor.None;
		}
		
		#if !ANDROID_UNITY
		if (SystemInfo.supportsGyroscope)
		{
			Debug.LogWarning("REAL (gyro) controls...");
			
	        Input.gyro.enabled = true;
	        Input.compass.enabled = true;			
			virtualTour = false;
			sensor = OrientationSensor.Gyro;
		}
		#endif
		else
		{
			Debug.LogWarning("REAL (accel) controls...");
			lowPassValue = Input.acceleration;
			lowPassCompassValue = Input.compass.magneticHeading;
			LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds; // tweakable
			
			sensor = OrientationSensor.Accel;
			virtualTour = false;
		}
		
		GetComponent<Camera>().fieldOfView = UIController.FOV;
		
		Debug.Log("Using camera FOV of " + GetComponent<Camera>().fieldOfView);
    }

    public static bool lockOrientation = false;	
	public static bool virtualTour = false;
	public static bool ignoreCompass = false;
	public static bool useAccelerometerLowPass = true;
	public static bool useCompassLowPass = true;
    void Update()
    {
		if (lockOrientation)// || virtualTour)
			return;
		
		if (Application.isEditor)
		{
			if (Input.GetKey("right"))
			{
				//transform.Rotate(Vector3.up * 30 * Time.deltaTime);
			}
			else if (Input.GetKey("left"))
			{
				//transform.Rotate(Vector3.up * -30 * Time.deltaTime);
			}

			if (Input.GetKey("up"))
			{
				transform.Rotate(Vector3.right * 30 * Time.deltaTime);
			}
			else if (Input.GetKey("down"))
			{
				transform.Rotate(Vector3.right * -30 * Time.deltaTime);
			}
			return;
		}
		
		if (sensor == OrientationSensor.Gyro)
		{
	        Quaternion gyroOrientation = Quaternion.Euler (90, 0, 0) * Input.gyro.attitude * Quaternion.Euler(0, 0, 90);

	        if (Input.compass.timestamp > _lastCompassUpdateTime)
	        {
	            _lastCompassUpdateTime = Input.compass.timestamp;

	            Vector3 gravity = Input.gyro.gravity.normalized;
	            Vector3 flatNorth = Input.compass.rawVector - 
	                Vector3.Dot(gravity, Input.compass.rawVector) * gravity;
	            _compassOrientation = Quaternion.Euler (180, 0, 0) * Quaternion.Inverse(Quaternion.LookRotation(flatNorth, -gravity)) * Quaternion.Euler (0, 0, 90);

				if (!ignoreCompass)
					_targetCorrection = _compassOrientation * Quaternion.Inverse(gyroOrientation);
	        }
        
	        if (Quaternion.Angle(_correction, _targetCorrection) > 45)
	            _correction = _targetCorrection;
	        else
	            _correction = Quaternion.Slerp(_correction, _targetCorrection, 0.02f);

	        transform.rotation = _correction * gyroOrientation;
			transform.eulerAngles = new Vector3(transform.eulerAngles.x-pitchOffsetPositiveUp, transform.eulerAngles.y, transform.eulerAngles.z);
		
			//transform.rotation = _compassOrientation;			
		}
		else if (sensor == OrientationSensor.Accel)
		{
			Vector3 accel = useAccelerometerLowPass ? LowPassFilterAccelerometer() : Input.acceleration;
			Vector3 r = Vector3.zero;
			
			r.x = accel.z >= 0 ? (-1 * (1 + accel.x) * 90) : (1+accel.x)*90;
			r.y = useCompassLowPass ? LowPassFilterCompass() : Input.compass.magneticHeading;
			transform.eulerAngles = r;
			
			//transform.eulerAngles = new Vector3(0, Input.compass.magneticHeading, 0);
		}
   }
   
   float AccelerometerUpdateInterval= 1f / 60f;
   float LowPassKernelWidthInSeconds = 0.05f;

   float LowPassFilterFactor;
   Vector3 lowPassValue = Vector3.zero;
   Vector3 LowPassFilterAccelerometer ()
   {
	   lowPassValue = Vector3.Lerp(lowPassValue, Input.acceleration, LowPassFilterFactor);
	   return lowPassValue;
   }

   float lowPassCompassValue = 0;
   float LowPassFilterCompass ()
   {
	   lowPassCompassValue = Mathf.LerpAngle(lowPassCompassValue, Input.compass.magneticHeading, LowPassFilterFactor);
	   return lowPassCompassValue;
   }

   void LateUpdate ()
   {
	   if (invertCamera)
	   {
		   transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 180);
	   }
	   else if (transform.eulerAngles.z != 0)
	   {
	   		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
	   }
	   
		
		ghost.rotation = transform.rotation;
		ghost.position = transform.position;
   }
}