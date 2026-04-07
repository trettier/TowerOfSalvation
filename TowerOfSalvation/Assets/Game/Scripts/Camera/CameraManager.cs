using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public CharactersZone target;
    public float followSpeed = 5f;
    public float cursorInfluence = 0.3f;
    public float maxCursorOffset = 3f;

    [Header("Boundaries")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private CameraState currentState;
    private Camera cam;
    private bool isInitialized = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = true;
        }
    }

    public void Initialize(CharactersZone target)
    {
        this.target = target;
        currentState = new IdleCamera();
        currentState.Enter(this);
        isInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!isInitialized || target == null)
            return;

        currentState.Update();
    }

    public void ChangeState(CameraState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    public void ChangeTarget(CharactersZone target)
    {
        this.target = target;
    }

    public void SetFollowState() => ChangeState(new FollowCamera());
    public void SetIdleState() => ChangeState(new IdleCamera());

    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    public void BoundsFromTilemap(Tilemap tilemap)
    {
        BoundsInt cellBounds = tilemap.cellBounds;
        Vector3 min = tilemap.CellToWorld(cellBounds.min);
        Vector3 max = tilemap.CellToWorld(cellBounds.max);

        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        minX = min.x + cameraWidth;
        maxX = max.x - cameraWidth;
        minY = min.y + cameraHeight;
        maxY = max.y - cameraHeight;
    }

    public Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }

    public Vector3 GetAverageCharactersPosition()
    {
        int characterCount = target.characters.Count;

        if (characterCount == 0)
            return target.transform.position + GetCursorOffset();

        Vector3 sum = Vector3.zero;
        foreach (var character in target.characters.Keys)
        {
            sum += character.transform.position;
        }

        return sum / target.characters.Count + GetCursorOffset();
    }
    public Vector3 GetCursorOffset()
    {
        if (cam == null) return Vector3.zero;

        // Получаем позицию курсора в мировых координатах
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Получаем текущую позицию камеры
        Vector3 cameraPos = transform.position;

        // Вычисляем направление от камеры к курсору
        Vector3 directionToCursor = mouseWorldPos - cameraPos;

        // Ограничиваем максимальное смещение
        float influence = Mathf.Min(directionToCursor.magnitude, maxCursorOffset);

        // Возвращаем смещение с учетом влияния
        return directionToCursor.normalized * influence * cursorInfluence;
    }

    public void Shake()
    {

    }
}

public abstract class CameraState
{
    protected CameraManager cameraManager;

    public virtual void Enter(CameraManager manager)
    {
        cameraManager = manager;
    }

    public virtual void Update() { }
    public virtual void Exit() { }
}

public class IdleCamera : CameraState
{
    private Vector3 velocity = Vector3.zero;

    public override void Update()
    {
        Vector3 targetPosition = GetTargetPosition();

        Vector3 smoothedPosition = Vector3.SmoothDamp(
            cameraManager.transform.position,
            targetPosition,
            ref velocity,
            1f / cameraManager.followSpeed
        );

        smoothedPosition = cameraManager.ClampPosition(smoothedPosition);
        cameraManager.transform.position = smoothedPosition;
    }

    private Vector3 GetTargetPosition()
    {

        Vector3 targetPos = cameraManager.GetAverageCharactersPosition();

        return new Vector3(
            targetPos.x,
            targetPos.y,
            cameraManager.transform.position.z
        );
    }

    public override void Exit()
    {
        base.Exit();
        velocity = Vector3.zero;
    }
}

public class FollowCamera : CameraState
{
    private Vector3 velocity = Vector3.zero;

    public override void Update()
    {
        Vector3 targetPosition = cameraManager.GetAverageCharactersPosition();

        targetPosition = new Vector3(
            targetPosition.x,
            targetPosition.y,
            cameraManager.transform.position.z
        );

        Vector3 smoothedPosition = Vector3.SmoothDamp(
            cameraManager.transform.position,
            targetPosition,
            ref velocity,
            1f / cameraManager.followSpeed
        );

        smoothedPosition = cameraManager.ClampPosition(smoothedPosition);
        cameraManager.transform.position = smoothedPosition;
    }
}