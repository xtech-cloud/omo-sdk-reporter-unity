using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OMO.SDK.NET;

public class Sample : MonoBehaviour
{

    public Text text;
    public InputField inputReport;
    public Button btnRun;
    public Button btnStop;

    private Reporter reporter = null;

    private byte[] data { get; set; }
    private Queue<string> log = new Queue<string>();
    private StringBuilder sb = new StringBuilder();

    private float timer = 0f;

    private float second = 0f;


    void Awake()
    {
        addLog("run reporter");
        inputReport.text = "127.0.0.1:18999";
        btnStop.gameObject.SetActive(false);
    }

    void Start()
    {
        ProtocolJSON.Request req = new ProtocolJSON.Request();
        req.head.msg = "ping";
        string json = ProtocolJSON.RequestToJSON(req);
        data = System.Text.Encoding.UTF8.GetBytes(json);

        btnRun.onClick.AddListener(() =>
        {
            btnRun.gameObject.SetActive(false);
            btnStop.gameObject.SetActive(true);
            reporter = ReporterMgr.NewReporter(string.Format("{0}", Time.deltaTime));
            reporter.Setup(inputReport.text.Split(':')[0], int.Parse(inputReport.text.Split(':')[1]));
            reporter.onReply = this.OnReply;
            reporter.onException = this.OnException;
            reporter.Run();
        });

        btnStop.onClick.AddListener(() =>
        {
            btnRun.gameObject.SetActive(true);
            btnStop.gameObject.SetActive(false);
            if (null != reporter)
            {
                reporter.Stop();
                reporter = null;
            }
        });
    }

    void Update()
    {
        text.text = printLog();

        if (timer > 1.0f)
        {
            second += 1f;
            if (null != reporter)
            {
                addLog("ping ... ");
                reporter.Report(data);
            }
            timer = 0f;
        }
        timer += Time.deltaTime;
    }

    void OnDisable()
    {
        if (null != reporter)
        {
            reporter.Stop();
        }
    }

    void OnReply(byte[] _data)
    {
        string reply = System.Text.Encoding.UTF8.GetString(_data);
        addLog(reply);
    }

    void OnException(System.Exception ex)
    {
        addLog(ex.Message);
    }

    void addLog(string _msg)
    {
        log.Enqueue(string.Format("{0} - {1}", second, _msg));
        if (log.Count > 20)
            log.Dequeue();
    }

    string printLog()
    {
        sb.Remove(0, sb.Length);
        foreach(string msg in log)
        {
            sb.AppendLine(msg);
        }
        return sb.ToString();
    }

}
