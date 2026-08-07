using UnityEngine;

public class PlayerCarInput : MonoBehaviour
{
    public int playerID = 1;

    private RCC_CarControllerV4 car;
    private RCC_Inputs inputs;
    private CarPowerUpManager manager;

    private void Awake()
    {
        car = GetComponent<RCC_CarControllerV4>();
        inputs = new RCC_Inputs();
        manager = GetComponent<CarPowerUpManager>();
    }

    private void Update()
    {
        if (playerID == 1)
        {
            float vertical = 0f;

            if (Input.GetKey(KeyCode.W))
                vertical = 1f;
            else if (Input.GetKey(KeyCode.S))
                vertical = -1f;

            float horizontal = 0f;

            if (Input.GetKey(KeyCode.A))
                horizontal = -1f;
            else if (Input.GetKey(KeyCode.D))
                horizontal = 1f;

            inputs.throttleInput = Mathf.Clamp01(vertical);
            inputs.brakeInput = Mathf.Clamp01(-vertical);
            inputs.steerInput = horizontal;

            inputs.handbrakeInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;
            inputs.boostInput = Input.GetKey(KeyCode.F) ? 1f : 0f;
        }
        else
        {
            float vertical = 0f;

            if (Input.GetKey(KeyCode.UpArrow))
                vertical = 1f;
            else if (Input.GetKey(KeyCode.DownArrow))
                vertical = -1f;

            float horizontal = 0f;

            if (Input.GetKey(KeyCode.LeftArrow))
                horizontal = -1f;
            else if (Input.GetKey(KeyCode.RightArrow))
                horizontal = 1f;

            inputs.throttleInput = Mathf.Clamp01(vertical);
            inputs.brakeInput = Mathf.Clamp01(-vertical);
            inputs.steerInput = horizontal;

            inputs.handbrakeInput = Input.GetKey(KeyCode.RightControl) ? 1f : 0f;
            inputs.boostInput = Input.GetKey(KeyCode.RightShift) ? 1f : 0f;
        }

        if (playerID == 1)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                manager.UsePowerUp();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                manager.UsePowerUp();
            }
        }

        car.OverrideInputs(inputs);
    }
}