    var m_Speed : float = 1;
    var m_XScale : float = 1;
    var m_YScale : float = 1;
     
    private var m_Pivot : Vector3;
    private var m_PivotOffset : Vector3;
    private var m_Phase : float;
    private var m_Invert : boolean = false;
    private var m_2PI : float = Mathf.PI * 2;
     
    function Start() {
        m_Pivot = transform.position;
    }
     
    function Update () {
        m_PivotOffset = Vector3.up * 2 * m_YScale;
       
        m_Phase += m_Speed * Time.deltaTime;
        if(m_Phase > m_2PI)
        {
            m_Invert = !m_Invert;
            m_Phase -= m_2PI;
        }
        if(m_Phase < 0) m_Phase += m_2PI;
       
        transform.position = m_Pivot + (m_Invert ? m_PivotOffset : Vector3.zero);
        transform.position.x += Mathf.Sin(m_Phase) * m_XScale;
        transform.position.y += Mathf.Cos(m_Phase) * (m_Invert ? -1 : 1) * m_YScale;
    }