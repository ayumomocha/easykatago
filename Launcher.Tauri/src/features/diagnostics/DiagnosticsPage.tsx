const checks = [
  { name: "settings.json 可读", status: "通过", detail: "installRoot = ." },
  { name: "profiles.json 可读", status: "通过", detail: "默认档案存在" },
  { name: "桥接进程路径", status: "待确认", detail: "等待首次命令调用" }
];

export function DiagnosticsPage() {
  return (
    <section>
      <h2>诊断</h2>
      <p>用于快速确认当前运行环境与数据文件健康状态。</p>
      <table>
        <thead>
          <tr>
            <th>检查项</th>
            <th>状态</th>
            <th>详情</th>
          </tr>
        </thead>
        <tbody>
          {checks.map((check) => (
            <tr key={check.name}>
              <td>{check.name}</td>
              <td>{check.status}</td>
              <td>{check.detail}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
