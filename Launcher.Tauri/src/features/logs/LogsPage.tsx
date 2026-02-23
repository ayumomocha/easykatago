const sampleLogs = [
  { level: "INFO", message: "安装流程已就绪。", time: "10:15:03" },
  { level: "WARN", message: "检测到旧版本缓存，将在下一次任务中清理。", time: "10:15:08" },
  { level: "INFO", message: "桥接连接状态正常。", time: "10:15:10" }
];

export function LogsPage() {
  return (
    <section>
      <h2>日志</h2>
      <p>集中查看安装、启动与桥接事件。</p>
      <ul>
        {sampleLogs.map((entry) => (
          <li key={`${entry.time}-${entry.message}`}>
            <strong>[{entry.time}]</strong> {entry.level} - {entry.message}
          </li>
        ))}
      </ul>
    </section>
  );
}
