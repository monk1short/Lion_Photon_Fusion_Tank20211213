using UnityEngine;
using Fusion;

/// <summary>
/// �Z�J���a���
/// �e�ᥪ�k����
/// �����P�o�g�l�u
/// </summary>
public class PlayerControl : NetworkBehaviour
{
    #region ���
    [Header("���ʳt��"), Range(0, 100)]
    public float speed = 7.5f;
    [Header("�o�g�l�u���j"), Range(0, 1.5f)]
    public float intervalFire = 0.35f;
    [Header("�l�u����")]
    public Bullet bullet;
    [Header("�l�u�ͦ���m")]
    public Transform pointFire;
    [Header("����")]
    public Transform traTower;

    /// <summary>
    /// �s�u���ⱱ�
    /// </summary>
    private NetworkCharacterController ncc;
    #endregion

    #region 
    /// <summary>
    /// �}�j���j�p�ɾ�
    /// </summary>
    public TickTimer interval { get; set; }
    #endregion 

    #region �ƥ�
    private void Awake()
    {
        ncc = GetComponent<NetworkCharacterController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �p�G �I�� ���� �W�� �]�t �l�u �N �R��
        if (collision.gameObject.name.Contains("�l�u")) Destroy(gameObject);
    }
    #endregion

    #region ��k
    /// <summary>
    /// Fusion �T�w��s�ƥ� ������ Unity Fixed Update
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        Move();
        Fire();
    }

    /// <summary>
    /// ����
    /// </summary>
    private void Move()
    {
        // �p�G �� ��J���
        if (GetInput(out NetworkInputData dataInput))
        {
            // �s�u���ⱱ�.����(�t�� * ��V * �s�u�@�ծɶ�)
            ncc.Move(speed * dataInput.direction * Runner.DeltaTime);

            // ���o�ƹ��y�СA�ñN Y ���w�P����@�˪������קK����n��
            Vector3 positionMouse = dataInput.positionMouse;
            positionMouse.y = traTower.position.y;
            // ���� �� �e��b�V = �ƹ� - �Z�J (�V�q)
            traTower.forward = positionMouse - transform.position;
        }
    }

    /// <summary>
    /// �}�j
    /// </summary>
    private void Fire()
    {
        if (GetInput(out NetworkInputData dataInput))                               // �p�G ���a����J���
        {
            if (interval.ExpiredOrNotRunning(Runner))                               // �p�G �}�j���j�p�ɾ� �L���Ϊ̨S���b����
            {
                if (dataInput.inputFire)                                            // �p�G ��J��ƬO�}�j����
                {
                    interval = TickTimer.CreateFromSeconds(Runner, intervalFire);   // �إ߭p�ɾ�

                    Runner.Spawn(                                                   // �s�u.�ͦ� (�s�u����D�y�СD���סD��J�v���D�ΦW�禡(���澹�D�ͦ�����) => {})
                        bullet,
                        pointFire.position,
                        pointFire.rotation,
                        Object.InputAuthority,
                        (runner, objectSpawn) =>
                        {
                        objectSpawn.GetComponent<Bullet>().Init();
                        });
                }
            }
        }
    }
    #endregion
}
