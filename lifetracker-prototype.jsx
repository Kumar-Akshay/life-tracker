import { useState, useEffect, useRef } from "react";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, AreaChart, Area, CartesianGrid } from "recharts";

/*
 * ═══════════════════════════════════════════════
 *  LIFETRACKER PRO — Editorial Edition
 *  Palette: Crail · Cloudy · Pampas · White
 *  Aesthetic: Kinfolk magazine meets productivity
 * ═══════════════════════════════════════════════
 */

const C = {
  crail: "#C15F3C", crailLight: "#D4795A", crailPale: "rgba(193,95,60,0.08)",
  crailGlow: "rgba(193,95,60,0.12)", crailMuted: "rgba(193,95,60,0.5)",
  cloudy: "#B1ADA1", cloudyLight: "#C8C4B9", cloudyPale: "rgba(177,173,161,0.15)",
  pampas: "#F4F3EE", white: "#FFFFFF",
  ink: "#2C2825", inkSoft: "#4A4541", inkMuted: "#7A7570", inkFaint: "#A8A29E",
  border: "rgba(177,173,161,0.25)", borderAccent: "rgba(193,95,60,0.2)",
  success: "#5B8C51", successPale: "rgba(91,140,81,0.1)",
  warning: "#C49A2D", warningPale: "rgba(196,154,45,0.1)",
  danger: "#C15050", dangerPale: "rgba(193,80,80,0.1)",
};

const catColors = ["#6B8EC7", "#C15F3C", "#C49A2D", "#B1ADA1", "#5B8C51", "#5CAFC7", "#4A7EC7", "#C76B8E", "#8E6BC7", "#7BA84A", "#4AC7A5", "#D4795A"];
const CATS = [
  { name: "Sleep", l: "S", col: catColors[0] }, { name: "Work", l: "W", col: catColors[1] }, { name: "Hobbies", l: "H", col: catColors[2] },
  { name: "Social", l: "O", col: catColors[3] }, { name: "Exercise", l: "E", col: catColors[4] }, { name: "Friends", l: "F", col: catColors[5] },
  { name: "Book", l: "B", col: catColors[6] }, { name: "Partner", l: "P", col: catColors[7] }, { name: "Family", l: "A", col: catColors[8] },
  { name: "Productive", l: "D", col: catColors[9] }, { name: "Travel", l: "T", col: catColors[10] }, { name: "Misc", l: "M", col: catColors[11] },
];
const lcm = { S: "#6B8EC7", W: "#C15F3C", H: "#C49A2D", E: "#5B8C51", F: "#5CAFC7", B: "#4A7EC7", P: "#C76B8E", G: "#D4795A", M: "#D4795A", T: "#4AC7A5", O: "#B1ADA1", A: "#8E6BC7", D: "#7BA84A" };
const gc = l => lcm[l] || C.cloudy;

const DAYS = [
  { dt: "Jan 25", dy: "Sat", h: ["M", "M", "S", "S", "S", "S", "S", "S", "W", "E", "E", "S", "W", "W", "W", "E", "W", "H", "F", "F", "F", "F", "W", "B"], wt: 91.2, sp: 0, hl: "Support office work, day one of fasting" },
  { dt: "Jan 26", dy: "Sun", h: ["M", "M", "M", "S", "S", "S", "S", "S", "S", "S", "S", "S", "S", "S", "S", "E", "M", "M", "S", "S", "S", "S", "M", "M"], wt: 90.8, sp: 24.5, hl: "Rest day — needed the downtime" },
  { dt: "Jan 27", dy: "Mon", h: ["M", "M", "M", "M", "S", "S", "S", "S", "S", "G", "W", "W", "W", "W", "H", "S", "S", "S", "S", "E", "M", "M", "M", "M"], wt: 90.0, sp: 17, hl: "Narender came, mixed day" },
  { dt: "Jan 28", dy: "Tue", h: ["M", "M", "S", "S", "S", "S", "S", "S", "S", "G", "W", "W", "W", "W", "W", "W", "W", "H", "H", "E", "E", "M", "M", "S"], wt: 89.5, sp: 0, hl: "Good productive work day" },
  { dt: "Jan 29", dy: "Wed", h: ["M", "M", "S", "S", "S", "S", "S", "S", "S", "G", "W", "W", "W", "W", "W", "W", "W", "S", "E", "E", "H", "H", "M", "M"], wt: 89.2, sp: 12, hl: "Solid work session, evening hobbies" },
  { dt: "Jan 30", dy: "Thu", h: ["M", "M", "M", "M", "S", "S", "S", "S", "S", "G", "W", "W", "W", "W", "W", "E", "S", "S", "S", "F", "F", "F", "M", "M"], wt: 89.0, sp: 35, hl: "Met friends in evening" },
  { dt: "Jan 31", dy: "Fri", h: ["M", "M", "S", "S", "S", "S", "S", "S", "S", "G", "W", "W", "W", "W", "H", "H", "E", "E", "S", "S", "B", "B", "M", "M"], wt: 88.8, sp: 8, hl: "End of month, reading time" },
];
const DASH = [{ n: "Sleep", v: 246 }, { n: "Work", v: 131 }, { n: "Social", v: 88 }, { n: "Exercise", v: 37 }, { n: "Book", v: 17 }, { n: "Productive", v: 34 }, { n: "Friends", v: 61 }, { n: "Family", v: 35 }];
const WKLY = [{ w: "W1", sleep: 56, work: 32, exercise: 8 }, { w: "W2", sleep: 54, work: 38, exercise: 10 }, { w: "W3", sleep: 58, work: 35, exercise: 7 }, { w: "W4", sleep: 52, work: 40, exercise: 12 }];

// ═══ UTILITY COMPONENTS ═══

function Reveal({ children, delay = 0, style = {} }) {
  const [v, setV] = useState(false);
  useEffect(() => { const t = setTimeout(() => setV(true), delay); return () => clearTimeout(t) }, [delay]);
  return <div style={{ transition: "all 0.7s cubic-bezier(0.16,1,0.3,1)", opacity: v ? 1 : 0, transform: v ? "translateY(0)" : "translateY(24px)", ...style }}>{children}</div>;
}

function Num({ value, suffix = "", prefix = "", delay = 0 }) {
  const [n, setN] = useState(0);
  const target = typeof value === "number" ? value : parseFloat(value) || 0;
  useEffect(() => {
    const d = setTimeout(() => {
      let start = 0; const dur = 800; const t0 = Date.now();
      const tick = () => {
        const p = Math.min((Date.now() - t0) / dur, 1); const ease = 1 - Math.pow(1 - p, 3);
        setN(Math.round(ease * target)); if (p < 1) requestAnimationFrame(tick)
      }; tick()
    }, delay);
    return () => clearTimeout(d);
  }, [target, delay]);
  return <span>{prefix}{n}{suffix}</span>;
}

const tt = { background: C.white, border: `1px solid ${C.border}`, borderRadius: 12, color: C.ink, fontSize: 12, fontFamily: "'JetBrains Mono',monospace", boxShadow: "0 8px 30px rgba(44,40,37,0.08)" };

// ═══ SECTION LABEL ═══
function SectionLabel({ children }) {
  return <div style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 20 }}>
    <div style={{ width: 28, height: 2, background: C.crail, borderRadius: 1 }} />
    <span style={{ fontSize: 11, fontWeight: 700, fontFamily: "'JetBrains Mono',monospace", color: C.crail, textTransform: "uppercase", letterSpacing: "0.18em" }}>{children}</span>
  </div>;
}

// ═══ CARD ═══
function Card({ children, accent = false, style = {} }) {
  return <div style={{ background: C.white, borderRadius: 16, border: `1px solid ${accent ? C.borderAccent : C.border}`, padding: 24, boxShadow: accent ? "0 8px 32px rgba(193,95,60,0.06), 0 1px 3px rgba(0,0,0,0.04)" : "0 4px 20px rgba(0,0,0,0.03), 0 1px 3px rgba(0,0,0,0.03)", transition: "box-shadow 0.3s, transform 0.3s", ...style }}>{children}</div>;
}

// ═══ NAV ICONS ═══
const Ic = {
  grid: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><rect x="3" y="3" width="7" height="7" rx="2" /><rect x="14" y="3" width="7" height="7" rx="2" /><rect x="14" y="14" width="7" height="7" rx="2" /><rect x="3" y="14" width="7" height="7" rx="2" /></svg>,
  bar: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><line x1="18" y1="20" x2="18" y2="10" /><line x1="12" y1="20" x2="12" y2="4" /><line x1="6" y1="20" x2="6" y2="14" /></svg>,
  dollar: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><line x1="12" y1="1" x2="12" y2="23" /><path d="M17 5H9.5a3.5 3.5 0 000 7h5a3.5 3.5 0 010 7H6" /></svg>,
  target: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><circle cx="12" cy="12" r="10" /><circle cx="12" cy="12" r="6" /><circle cx="12" cy="12" r="2" /></svg>,
  pulse: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><polyline points="22,12 18,12 15,21 9,3 6,12 2,12" /></svg>,
  check: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><path d="M22 11.08V12a10 10 0 11-5.93-9.14" /><polyline points="22,4 12,14.01 9,11.01" /></svg>,
  edit: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7" /><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z" /></svg>,
  book: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><path d="M4 19.5A2.5 2.5 0 016.5 17H20" /><path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z" /></svg>,
  play: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><polygon points="5,3 19,12 5,21" /></svg>,
  zap: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><polygon points="13,2 3,14 12,14 11,22 21,10 12,10" /></svg>,
  gear: <svg width="19" height="19" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7"><circle cx="12" cy="12" r="3" /><path d="M19.4 15a1.65 1.65 0 00.33 1.82l.06.06a2 2 0 010 2.83 2 2 0 01-2.83 0l-.06-.06a1.65 1.65 0 00-1.82-.33 1.65 1.65 0 00-1 1.51V21a2 2 0 01-4 0v-.09A1.65 1.65 0 009 19.4a1.65 1.65 0 00-1.82.33l-.06.06a2 2 0 01-2.83-2.83l.06-.06A1.65 1.65 0 004.68 15a1.65 1.65 0 00-1.51-1H3a2 2 0 010-4h.09A1.65 1.65 0 004.6 9a1.65 1.65 0 00-.33-1.82l-.06-.06a2 2 0 012.83-2.83l.06.06A1.65 1.65 0 009 4.68a1.65 1.65 0 001-1.51V3a2 2 0 014 0v.09a1.65 1.65 0 001 1.51 1.65 1.65 0 001.82-.33l.06-.06a2 2 0 012.83 2.83l-.06.06A1.65 1.65 0 0019.4 9a1.65 1.65 0 001.51 1H21a2 2 0 010 4h-.09a1.65 1.65 0 00-1.51 1z" /></svg>,
};

// ═══════════════════════════════════════════
// SCREEN: DAILY LOGGER
// ═══════════════════════════════════════════
function DailyLogScreen() {
  const [si, setSi] = useState(3); const [ec, setEc] = useState(null); const [af, setAf] = useState("idle");
  const d = DAYS[si];
  return (<div>
    <Reveal><div style={{ marginBottom: 32 }}>
      <h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: 0, lineHeight: 1.1 }}>Daily <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Logger</span></h1>
      <p style={{ fontSize: 15, color: C.inkMuted, fontFamily: "'Source Sans 3',sans-serif", margin: "10px 0 0", fontWeight: 400 }}>Tap a cell to reassign · Auto-fill learns your patterns</p>
    </div></Reveal>

    <Reveal delay={60}><div style={{ display: "flex", gap: 8, marginBottom: 22, alignItems: "center" }}>
      {DAYS.map((dd, i) => <button key={i} onClick={() => { setSi(i); setAf("idle"); setEc(null) }} style={{
        padding: "10px 15px", borderRadius: 12, cursor: "pointer", transition: "all 0.3s", fontFamily: "'JetBrains Mono',monospace", fontSize: 11,
        border: i === si ? `2px solid ${C.crail}` : `1px solid ${C.border}`,
        background: i === si ? C.crailPale : C.white, color: i === si ? C.crail : C.inkMuted, fontWeight: i === si ? 700 : 400,
        boxShadow: i === si ? "0 4px 16px rgba(193,95,60,0.12)" : "none", transform: i === si ? "translateY(-2px)" : "none",
      }}><div style={{ fontWeight: 700 }}>{dd.dy}</div><div style={{ fontSize: 9, opacity: 0.6, marginTop: 2 }}>{dd.dt}</div></button>)}
      <div style={{ flex: 1 }} />
      <button onClick={() => { setAf("loading"); setTimeout(() => setAf("done"), 1600) }} style={{
        padding: "12px 28px", borderRadius: 12, border: "none", cursor: "pointer", fontFamily: "'JetBrains Mono',monospace", fontSize: 12, fontWeight: 700,
        background: af === "done" ? C.successPale : af === "loading" ? C.crailPale : `linear-gradient(135deg,${C.crail},${C.crailLight})`,
        color: af === "done" ? C.success : af === "loading" ? C.crail : C.white,
        boxShadow: af === "idle" ? `0 4px 20px rgba(193,95,60,0.3)` : "none", transition: "all 0.4s", letterSpacing: "0.02em",
      }}>{af === "loading" ? "◌ Analyzing..." : af === "done" ? "✓ Done" : "✦ Auto-Fill"}</button>
    </div></Reveal>

    <Reveal delay={120}><Card accent style={{ marginBottom: 22, position: "relative", overflow: "hidden", padding: 20 }}>
      {[0, 12].map(off => <div key={off}>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(12,1fr)", gap: 3, marginBottom: 3 }}>
          {[...Array(12)].map((_, i) => <div key={i} style={{ textAlign: "center", fontSize: 8, color: C.inkFaint, fontFamily: "'JetBrains Mono',monospace", padding: "2px 0", fontWeight: 600 }}>{String(i + off).padStart(2, "0")}</div>)}
        </div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(12,1fr)", gap: 5, marginBottom: off === 0 ? 16 : 0 }}>
          {d.h.slice(off, off + 12).map((h, i) => {
            const idx = i + off; const isE = ec === idx; const col = gc(h);
            return <div key={idx} onClick={() => setEc(isE ? null : idx)} style={{
              height: 50, borderRadius: 12, background: col, display: "flex", alignItems: "center", justifyContent: "center",
              cursor: "pointer", fontSize: 14, fontWeight: 800, color: "#fff", fontFamily: "'JetBrains Mono',monospace",
              transition: "all 0.25s cubic-bezier(0.34,1.56,0.64,1)",
              transform: isE ? "scale(1.22) translateY(-6px)" : "scale(1)",
              boxShadow: isE ? `0 12px 28px ${col}55, 0 0 0 3px ${C.white}, 0 0 0 5px ${col}40` : `0 3px 8px ${col}18`,
              position: "relative", zIndex: isE ? 10 : 1, textShadow: "0 1px 3px rgba(0,0,0,0.25)",
            }}>
              {h}
              {af === "done" && <div style={{ position: "absolute", top: -4, right: -4, width: 10, height: 10, borderRadius: "50%", background: idx % 4 === 0 ? C.warning : C.success, border: `2.5px solid ${C.white}`, boxShadow: `0 2px 6px ${idx % 4 === 0 ? C.warning : C.success}50` }} />}
            </div>
          })}
        </div>
      </div>)}
      {af === "loading" && <div style={{ position: "absolute", inset: 0, borderRadius: 16, background: `linear-gradient(90deg,transparent,${C.crailPale},transparent)`, animation: "shimmer 1.3s ease infinite", pointerEvents: "none", zIndex: 20 }} />}
    </Card></Reveal>

    {ec !== null && <Reveal><Card accent style={{ marginBottom: 20 }}>
      <div style={{ fontSize: 10, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.15em", marginBottom: 12 }}>Select category for {String(ec).padStart(2, "0")}:00</div>
      <div style={{ display: "flex", flexWrap: "wrap", gap: 8 }}>
        {CATS.map((c, i) => <button key={i} onClick={() => setEc(null)} style={{ padding: "8px 14px", borderRadius: 10, cursor: "pointer", background: c.col + "12", border: `1px solid ${c.col}28`, color: c.col, fontFamily: "'JetBrains Mono',monospace", fontSize: 11, fontWeight: 700, transition: "all 0.2s" }}>
          <span style={{ marginRight: 5, fontWeight: 800 }}>{c.l}</span>{c.name}
        </button>)}
      </div>
    </Card></Reveal>}

    <Reveal delay={200}><div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: 14, marginBottom: 22 }}>
      {[{ l: "Weight", v: `${d.wt}`, u: "kg", ic: "⚖" }, { l: "Spent", v: `$${d.sp}`, u: "", ic: "◎" }, { l: "Hours", v: "24/24", u: "", ic: "●" }].map((m, i) =>
        <Card key={i}><div style={{ display: "flex", justifyContent: "space-between", alignItems: "start" }}>
          <div><div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", textTransform: "uppercase", letterSpacing: "0.12em", marginBottom: 6 }}>{m.l}</div>
            <div style={{ fontSize: 26, fontWeight: 700, color: C.ink, fontFamily: "'Libre Baskerville',serif" }}>{m.v}<span style={{ fontSize: 13, fontWeight: 400, color: C.inkMuted, marginLeft: 3 }}>{m.u}</span></div></div>
          <span style={{ fontSize: 24, opacity: 0.15 }}>{m.ic}</span>
        </div></Card>)}
    </div></Reveal>

    <Reveal delay={280}><Card accent>
      <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 8 }}>
        <div style={{ width: 3, height: 20, borderRadius: 2, background: C.crail }} />
        <span style={{ fontSize: 10, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.15em" }}>Highlight</span>
      </div>
      <p style={{ fontSize: 19, color: C.ink, fontFamily: "'Libre Baskerville',serif", lineHeight: 1.6, margin: 0, fontStyle: "italic", fontWeight: 400 }}>"{d.hl}"</p>
    </Card></Reveal>
  </div>);
}

// ═══════════════════════════════════════════
// SCREEN: DASHBOARD
// ═══════════════════════════════════════════
function DashboardScreen() {
  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 6px" }}>Your <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Dashboard</span></h1>
      <p style={{ fontSize: 15, color: C.inkMuted, fontFamily: "'Source Sans 3',sans-serif", margin: "0 0 28px" }}>Analytics, trends, and life insights at a glance</p></Reveal>

    <Reveal delay={60}><div style={{ display: "grid", gridTemplateColumns: "repeat(4,1fr)", gap: 14, marginBottom: 28 }}>
      {[{ l: "Days Logged", v: 28, s: "", d: "▲ 4 this week" }, { l: "Total Hours", v: 664, s: "h", d: "100% coverage" }, { l: "Avg Sleep", v: 8.8, s: "h", d: "▲ 0.3h" }, { l: "Spent", v: 373, s: "", d: "Under budget", p: "$" }].map((k, i) =>
        <Card key={i}><div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", textTransform: "uppercase", letterSpacing: "0.12em", marginBottom: 8 }}>{k.l}</div>
          <div style={{ fontSize: 30, fontWeight: 700, color: C.ink, fontFamily: "'Libre Baskerville',serif" }}><Num value={k.v} prefix={k.p || ""} suffix={k.s} delay={200 + i * 100} /></div>
          <div style={{ fontSize: 10, color: C.crail, fontFamily: "'JetBrains Mono',monospace", marginTop: 6, fontWeight: 600 }}>{k.d}</div></Card>)}
    </div></Reveal>

    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginBottom: 22 }}>
      <Reveal delay={150}><Card>
        <SectionLabel>Distribution</SectionLabel>
        <ResponsiveContainer width="100%" height={240}>
          <PieChart><Pie data={DASH} cx="50%" cy="50%" innerRadius={58} outerRadius={95} dataKey="v" stroke={C.white} strokeWidth={3} paddingAngle={2}>
            {DASH.map((_, i) => <Cell key={i} fill={catColors[i]} />)}</Pie><Tooltip contentStyle={tt} formatter={v => `${v}h`} /></PieChart>
        </ResponsiveContainer>
      </Card></Reveal>
      <Reveal delay={200}><Card>
        <SectionLabel>By Category</SectionLabel>
        <ResponsiveContainer width="100%" height={240}>
          <BarChart data={DASH} layout="vertical" margin={{ left: 60 }}>
            <XAxis type="number" tick={{ fill: C.inkMuted, fontSize: 10, fontFamily: "'JetBrains Mono',monospace" }} axisLine={{ stroke: C.border }} />
            <YAxis type="category" dataKey="n" tick={{ fill: C.inkSoft, fontSize: 11, fontFamily: "'Source Sans 3',sans-serif" }} width={58} axisLine={false} tickLine={false} />
            <Tooltip contentStyle={tt} /><Bar dataKey="v" radius={[0, 8, 8, 0]}>{DASH.map((_, i) => <Cell key={i} fill={catColors[i]} />)}</Bar>
          </BarChart>
        </ResponsiveContainer>
      </Card></Reveal>
    </div>

    <Reveal delay={280}><Card>
      <SectionLabel>Weekly Trends</SectionLabel>
      <ResponsiveContainer width="100%" height={200}>
        <AreaChart data={WKLY}><CartesianGrid strokeDasharray="3 3" stroke={C.border} />
          <XAxis dataKey="w" tick={{ fill: C.inkMuted, fontSize: 11, fontFamily: "'JetBrains Mono',monospace" }} />
          <YAxis tick={{ fill: C.inkMuted, fontSize: 10 }} axisLine={false} />
          <Tooltip contentStyle={tt} />
          <Area type="monotone" dataKey="sleep" stackId="1" stroke="#6B8EC7" fill="#6B8EC7" fillOpacity={0.3} />
          <Area type="monotone" dataKey="work" stackId="1" stroke={C.crail} fill={C.crail} fillOpacity={0.3} />
          <Area type="monotone" dataKey="exercise" stackId="1" stroke="#5B8C51" fill="#5B8C51" fillOpacity={0.3} />
        </AreaChart>
      </ResponsiveContainer>
    </Card></Reveal>
  </div>);
}

// ═══════════════════════════════════════════
// SCREEN: SPENDING
// ═══════════════════════════════════════════
function SpendingScreen() {
  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 6px" }}>Spending & <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Budget</span></h1>
      <p style={{ fontSize: 15, color: C.inkMuted, fontFamily: "'Source Sans 3',sans-serif", margin: "0 0 28px" }}>Track every dollar, stay in control</p></Reveal>

    <Reveal delay={60}><Card accent style={{ marginBottom: 22 }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "baseline", marginBottom: 14 }}>
        <span style={{ fontSize: 11, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", textTransform: "uppercase", letterSpacing: "0.12em" }}>February Budget</span>
        <span style={{ fontSize: 22, fontWeight: 700, color: C.crail, fontFamily: "'Libre Baskerville',serif" }}>$280 <span style={{ fontSize: 14, fontWeight: 400, color: C.inkMuted }}>/ $500</span></span>
      </div>
      <div style={{ height: 14, borderRadius: 7, background: C.pampas, overflow: "hidden" }}>
        <div style={{ height: "100%", width: "56%", borderRadius: 7, background: `linear-gradient(90deg,${C.crail},${C.crailLight})`, transition: "width 1.5s cubic-bezier(0.16,1,0.3,1)", boxShadow: `0 0 16px ${C.crail}30` }} />
      </div>
      <div style={{ display: "flex", justifyContent: "space-between", marginTop: 8 }}>
        <span style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace" }}>56% used</span>
        <span style={{ fontSize: 10, color: C.success, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700 }}>$220 remaining</span>
      </div>
    </Card></Reveal>

    <Reveal delay={120}><div style={{ display: "grid", gridTemplateColumns: "repeat(3,1fr)", gap: 14, marginBottom: 22 }}>
      {[{ l: "Avg / Month", v: "$31" }, { l: "Highest", v: "FEB $280" }, { l: "Year Total", v: "$373" }].map((m, i) =>
        <Card key={i}><div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", textTransform: "uppercase", letterSpacing: "0.12em", marginBottom: 6 }}>{m.l}</div>
          <div style={{ fontSize: 24, fontWeight: 700, color: C.ink, fontFamily: "'Libre Baskerville',serif" }}>{m.v}</div></Card>)}
    </div></Reveal>

    <Reveal delay={200}><Card style={{ marginBottom: 22 }}>
      <SectionLabel>Monthly Spending</SectionLabel>
      <ResponsiveContainer width="100%" height={180}>
        <BarChart data={[{ m: "JAN", a: 93 }, { m: "FEB", a: 280 }, { m: "MAR", a: 45 }, { m: "APR", a: 0 }]}>
          <XAxis dataKey="m" tick={{ fill: C.inkSoft, fontSize: 11 }} axisLine={{ stroke: C.border }} /><YAxis tick={{ fill: C.inkMuted, fontSize: 10 }} axisLine={false} />
          <Tooltip contentStyle={tt} /><Bar dataKey="a" fill={C.crail} radius={[8, 8, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </Card></Reveal>

    <Reveal delay={280}><Card>
      <SectionLabel>Transactions</SectionLabel>
      {[{ dt: "Feb 9", d: "DJI Camera (Installment)", a: "$498", tg: "Tech" }, { dt: "Jan 27", d: "Send money — Pakistan", a: "$4,000", tg: "Transfer" }, { dt: "Jan 26", d: "Groceries", a: "$24.50", tg: "Food" }, { dt: "Jan 27", d: "Transport", a: "$17", tg: "Travel" }].map((t, i) =>
        <div key={i} style={{ display: "flex", alignItems: "center", justifyContent: "space-between", padding: "14px 0", borderBottom: i < 3 ? `1px solid ${C.border}` : "none" }}>
          <div><div style={{ color: C.ink, fontSize: 14, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 600 }}>{t.d}</div>
            <div style={{ display: "flex", gap: 8, marginTop: 4 }}><span style={{ color: C.inkMuted, fontSize: 10, fontFamily: "'JetBrains Mono',monospace" }}>{t.dt}</span>
              <span style={{ fontSize: 9, padding: "2px 8px", borderRadius: 6, background: C.crailPale, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 600 }}>{t.tg}</span></div></div>
          <span style={{ color: C.danger, fontWeight: 700, fontSize: 15, fontFamily: "'JetBrains Mono',monospace" }}>{t.a}</span>
        </div>)}
    </Card></Reveal>
  </div>);
}

// ═══════════════════════════════════════════
// SCREEN: GOALS
// ═══════════════════════════════════════════
function GoalsScreen() {
  const gs = [{ ty: "Daily", it: [{ n: "Sleep ≥ 7h", p: 100 }, { n: "Exercise ≥ 1h", p: 100 }, { n: "Spend ≤ $20", p: 100 }, { n: "Read 30 min", p: 0 }] }, { ty: "Monthly", it: [{ n: "Read 4 books", p: 50 }, { n: "Workout 20+ days", p: 65 }, { n: "Save $500", p: 44 }] }, { ty: "Yearly", it: [{ n: "Travel 3 countries", p: 0 }, { n: "Lose 5 kg", p: 48 }, { n: "12 courses", p: 17 }] }];
  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 28px" }}>Your <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Goals</span></h1></Reveal>
    <Reveal delay={60}><div style={{ display: "grid", gridTemplateColumns: "repeat(3,1fr)", gap: 14, marginBottom: 30 }}>
      {[{ l: "Exercise", v: "5 days", e: "🔥" }, { l: "Logging", v: "28 days", e: "◉" }, { l: "Budget", v: "12 days", e: "◎" }].map((s, i) =>
        <Card key={i} accent><div style={{ textAlign: "center", padding: "8px 0" }}>
          <div style={{ fontSize: 32, marginBottom: 6 }}>{s.e}</div>
          <div style={{ fontSize: 28, fontWeight: 700, color: C.crail, fontFamily: "'Libre Baskerville',serif" }}>{s.v}</div>
          <div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", textTransform: "uppercase", letterSpacing: "0.12em", marginTop: 4 }}>{s.l} Streak</div>
        </div></Card>)}
    </div></Reveal>
    {gs.map((g, gi) => <Reveal key={gi} delay={120 + gi * 80}><div style={{ marginBottom: 28 }}>
      <SectionLabel>{g.ty} Goals</SectionLabel>
      {g.it.map((gl, i) => <Card key={i} style={{ marginBottom: 10 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 10 }}>
          <span style={{ color: C.ink, fontSize: 14, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 600 }}>{gl.n}</span>
          <span style={{ color: gl.p === 100 ? C.success : gl.p > 40 ? C.crail : C.inkMuted, fontSize: 14, fontWeight: 800, fontFamily: "'JetBrains Mono',monospace" }}>{gl.p}%</span>
        </div>
        <div style={{ height: 8, borderRadius: 4, background: C.pampas, overflow: "hidden" }}>
          <div style={{
            height: "100%", width: `${gl.p}%`, borderRadius: 4, transition: "width 1.2s cubic-bezier(0.16,1,0.3,1)",
            background: gl.p === 100 ? C.success : `linear-gradient(90deg,${C.crail},${C.crailLight})`
          }} />
        </div>
      </Card>)}
    </div></Reveal>)}
  </div>);
}

// ═══════════════════════════════════════════
// ★ NEW SCREEN: LIFE PULSE (Wellness Correlations)
// ═══════════════════════════════════════════
function LifePulseScreen() {
  const scores = [{ l: "Sleep Quality", v: 82, col: "#6B8EC7" }, { l: "Physical Activity", v: 68, col: "#5B8C51" }, { l: "Productivity", v: 78, col: C.crail }, { l: "Social Balance", v: 55, col: "#5CAFC7" }, { l: "Financial Health", v: 91, col: "#C49A2D" }, { l: "Learning", v: 42, col: "#8E6BC7" }];
  const overall = Math.round(scores.reduce((a, s) => a + s.v, 0) / scores.length);
  const correlations = [{ a: "Sleep ≥ 8h", b: "Productivity ▲ 23%", type: "positive" }, { a: "Exercise days", b: "Mood ▲ 31%", type: "positive" }, { a: "Spend > $50/day", b: "Stress ▲ 18%", type: "negative" }, { a: "Social < 1h", b: "Focus ▲ 15%", type: "neutral" }];

  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 6px" }}>Life <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Pulse</span></h1>
      <p style={{ fontSize: 15, color: C.inkMuted, fontFamily: "'Source Sans 3',sans-serif", margin: "0 0 28px" }}>Holistic wellness score — how every dimension of life connects</p></Reveal>

    <div style={{ display: "grid", gridTemplateColumns: "280px 1fr", gap: 20, marginBottom: 28 }}>
      <Reveal delay={60}><Card accent style={{ textAlign: "center", padding: 32 }}>
        <svg width="180" height="180" viewBox="0 0 180 180" style={{ margin: "0 auto", display: "block" }}>
          <circle cx="90" cy="90" r="78" fill="none" stroke={C.pampas} strokeWidth="12" />
          <circle cx="90" cy="90" r="78" fill="none" stroke={C.crail} strokeWidth="12"
            strokeDasharray={`${(overall / 100) * 490} 490`} strokeLinecap="round" transform="rotate(-90 90 90)"
            style={{ transition: "stroke-dasharray 2s cubic-bezier(0.16,1,0.3,1)", filter: `drop-shadow(0 0 12px ${C.crail}40)` }} />
          <text x="90" y="82" textAnchor="middle" fill={C.ink} fontSize="48" fontWeight="700" fontFamily="'Libre Baskerville',serif"><Num value={overall} delay={300} /></text>
          <text x="90" y="106" textAnchor="middle" fill={C.inkMuted} fontSize="12" fontFamily="'JetBrains Mono',monospace">/ 100</text>
        </svg>
        <div style={{ fontSize: 11, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.15em", marginTop: 14 }}>Overall Life Score</div>
      </Card></Reveal>

      <Reveal delay={120}><Card>
        <SectionLabel>Dimension Scores</SectionLabel>
        <div style={{ display: "grid", gap: 14 }}>
          {scores.map((s, i) => <div key={i}>
            <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 6 }}>
              <span style={{ fontSize: 13, color: C.ink, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 600 }}>{s.l}</span>
              <span style={{ fontSize: 13, color: s.col, fontFamily: "'JetBrains Mono',monospace", fontWeight: 800 }}>{s.v}</span>
            </div>
            <div style={{ height: 8, borderRadius: 4, background: C.pampas, overflow: "hidden" }}>
              <div style={{ height: "100%", width: `${s.v}%`, borderRadius: 4, background: s.col, transition: "width 1.5s cubic-bezier(0.16,1,0.3,1)", transitionDelay: `${i * 100}ms` }} />
            </div>
          </div>)}
        </div>
      </Card></Reveal>
    </div>

    <Reveal delay={220}><Card>
      <SectionLabel>Discovered Correlations</SectionLabel>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
        {correlations.map((c, i) => <div key={i} style={{ padding: 16, borderRadius: 12, background: c.type === "positive" ? C.successPale : c.type === "negative" ? C.dangerPale : C.cloudyPale, border: `1px solid ${c.type === "positive" ? `${C.success}20` : c.type === "negative" ? `${C.danger}20` : C.border}` }}>
          <div style={{ fontSize: 12, color: C.ink, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 700, marginBottom: 4 }}>When {c.a}</div>
          <div style={{ fontSize: 13, color: c.type === "positive" ? C.success : c.type === "negative" ? C.danger : C.inkSoft, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700 }}>{c.b}</div>
        </div>)}
      </div>
    </Card></Reveal>
  </div>);
}

// ═══════════════════════════════════════════
// ★ NEW SCREEN: HABIT TRACKER
// ═══════════════════════════════════════════
function HabitScreen() {
  const habits = [
    { name: "Meditate", streak: 12, data: [1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1] },
    { name: "No Sugar", streak: 5, data: [0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1] },
    { name: "Read 30min", streak: 8, data: [1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1] },
    { name: "Exercise", streak: 5, data: [1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1] },
    { name: "Journal", streak: 3, data: [0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1] },
    { name: "No Phone Before Bed", streak: 7, data: [1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1] },
  ];
  const dayLabels = Array.from({ length: 28 }, (_, i) => i + 1);

  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 6px" }}>Habit <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Tracker</span></h1>
      <p style={{ fontSize: 15, color: C.inkMuted, fontFamily: "'Source Sans 3',sans-serif", margin: "0 0 28px" }}>Build consistency — every checked day compounds into transformation</p></Reveal>

    <Reveal delay={60}><div style={{ display: "grid", gridTemplateColumns: "repeat(3,1fr)", gap: 14, marginBottom: 28 }}>
      {[{ l: "Active Habits", v: "6" }, { l: "Best Streak", v: "12 days" }, { l: "Completion Rate", v: "71%" }].map((m, i) =>
        <Card key={i}><div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", textTransform: "uppercase", letterSpacing: "0.12em", marginBottom: 6 }}>{m.l}</div>
          <div style={{ fontSize: 26, fontWeight: 700, color: C.ink, fontFamily: "'Libre Baskerville',serif" }}>{m.v}</div></Card>)}
    </div></Reveal>

    <Reveal delay={120}><Card style={{ overflowX: "auto" }}>
      <SectionLabel>January Heatmap</SectionLabel>
      <div style={{ display: "grid", gap: 6, minWidth: 600 }}>
        {/* Day numbers header */}
        <div style={{ display: "grid", gridTemplateColumns: "130px repeat(28,1fr)", gap: 3, alignItems: "center" }}>
          <div />
          {dayLabels.map(d => <div key={d} style={{ textAlign: "center", fontSize: 8, color: C.inkFaint, fontFamily: "'JetBrains Mono',monospace", fontWeight: 600 }}>{d}</div>)}
        </div>
        {habits.map((h, hi) => <div key={hi} style={{ display: "grid", gridTemplateColumns: "130px repeat(28,1fr)", gap: 3, alignItems: "center" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <span style={{ fontSize: 13, color: C.ink, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 600 }}>{h.name}</span>
            <span style={{ fontSize: 9, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700 }}>{h.streak}d</span>
          </div>
          {h.data.map((v, di) => <div key={di} style={{
            width: "100%", aspectRatio: "1", borderRadius: 4, transition: "all 0.2s", transitionDelay: `${di * 15}ms`,
            background: v ? C.crail : C.pampas, opacity: v ? 0.2 + 0.8 * (di / 28) : 0.5,
            boxShadow: v && di > 24 ? `0 0 6px ${C.crail}30` : "none",
          }} />)}
        </div>)}
      </div>
    </Card></Reveal>
  </div>);
}

// ═══════════════════════════════════════════
// ★ NEW SCREEN: WEEKLY REVIEW
// ═══════════════════════════════════════════
function WeeklyReviewScreen() {
  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 6px" }}>Weekly <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Review</span></h1>
      <p style={{ fontSize: 15, color: C.inkMuted, fontFamily: "'Source Sans 3',sans-serif", margin: "0 0 28px" }}>Reflect, recalibrate, and plan the week ahead</p></Reveal>

    <Reveal delay={60}><Card accent style={{ marginBottom: 22, borderLeft: `4px solid ${C.crail}` }}>
      <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 14 }}>
        <span style={{ fontSize: 10, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.15em" }}>AI Summary — Week 4, January</span>
      </div>
      <p style={{ fontSize: 16, color: C.ink, fontFamily: "'Libre Baskerville',serif", lineHeight: 1.8, margin: 0, fontStyle: "italic" }}>
        "Strong week for productivity with 40h of focused work — your highest this month. Exercise dropped slightly to 12h but you maintained your 5-day streak. Social time was lower than average; consider scheduling friend time next week. Weight trend is excellent at -2.4 kg for the month."
      </p>
    </Card></Reveal>

    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginBottom: 22 }}>
      <Reveal delay={120}><Card>
        <SectionLabel>Week at a Glance</SectionLabel>
        <div style={{ display: "grid", gap: 12 }}>
          {[{ l: "Work Hours", v: "40h", d: "▲ 5h vs W3", col: C.crail }, { l: "Sleep Avg", v: "7.4h", d: "▼ 0.4h", col: "#6B8EC7" }, { l: "Exercise", v: "12h", d: "▲ 5h vs W3", col: C.success }, { l: "Spending", v: "$55", d: "Under $75 budget", col: C.warning }].map((r, i) =>
            <div key={i} style={{ display: "flex", justifyContent: "space-between", alignItems: "center", padding: "10px 0", borderBottom: i < 3 ? `1px solid ${C.border}` : "none" }}>
              <span style={{ fontSize: 13, color: C.ink, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 600 }}>{r.l}</span>
              <div style={{ textAlign: "right" }}><div style={{ fontSize: 18, fontWeight: 700, color: r.col, fontFamily: "'Libre Baskerville',serif" }}>{r.v}</div>
                <div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace" }}>{r.d}</div></div>
            </div>)}
        </div>
      </Card></Reveal>

      <Reveal delay={180}><Card>
        <SectionLabel>Reflection</SectionLabel>
        {[{ q: "What went well?", a: "Consistent early mornings, maintained exercise streak, shipped two PRs" }, { q: "What could improve?", a: "Less social media scrolling, need to read more, skipped journaling twice" }, { q: "Next week's focus?", a: "Finish Clean Architecture book, prep for system design interview" }].map((r, i) =>
          <div key={i} style={{ marginBottom: i < 2 ? 18 : 0 }}>
            <div style={{ fontSize: 11, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.1em", marginBottom: 6 }}>{r.q}</div>
            <p style={{ fontSize: 14, color: C.ink, fontFamily: "'Source Sans 3',sans-serif", lineHeight: 1.6, margin: 0 }}>{r.a}</p>
          </div>)}
      </Card></Reveal>
    </div>

    <Reveal delay={240}><Card>
      <SectionLabel>Goals Progress This Week</SectionLabel>
      <div style={{ display: "grid", gridTemplateColumns: "repeat(4,1fr)", gap: 12 }}>
        {[{ n: "Exercise 5 days", p: 100, c: C.success }, { n: "Read 4 chapters", p: 75, c: C.crail }, { n: "Spend < $75", p: 100, c: C.success }, { n: "Meditate daily", p: 57, c: C.warning }].map((g, i) =>
          <div key={i} style={{ textAlign: "center", padding: 16, borderRadius: 14, background: g.p === 100 ? C.successPale : C.pampas, border: `1px solid ${g.p === 100 ? `${C.success}15` : C.border}` }}>
            <svg width="64" height="64" viewBox="0 0 64 64" style={{ margin: "0 auto 8px", display: "block" }}>
              <circle cx="32" cy="32" r="27" fill="none" stroke={C.pampas} strokeWidth="5" />
              <circle cx="32" cy="32" r="27" fill="none" stroke={g.c} strokeWidth="5" strokeDasharray={`${(g.p / 100) * 170} 170`} strokeLinecap="round" transform="rotate(-90 32 32)" style={{ transition: "stroke-dasharray 1.5s cubic-bezier(0.16,1,0.3,1)" }} />
              <text x="32" y="36" textAnchor="middle" fill={C.ink} fontSize="14" fontWeight="700" fontFamily="'JetBrains Mono',monospace">{g.p}%</text>
            </svg>
            <div style={{ fontSize: 11, color: C.inkSoft, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 600 }}>{g.n}</div>
          </div>)}
      </div>
    </Card></Reveal>
  </div>);
}

// ═══════════════════════════════════════════
// SCREEN: VIDEOS
// ═══════════════════════════════════════════
function VideosScreen() {
  const kb = { "To Start": [{ t: "System Design Interview", p: "YouTube", d: "2h 30m", tg: ["Architecture"], pr: "High" }, { t: "Advanced PostgreSQL", p: "Udemy", d: "8h", tg: ["Database"], pr: "Med" }], "In Progress": [{ t: "Clean Architecture .NET", p: "Pluralsight", d: "4h", pg: 65, tg: ["C#"], pr: "High" }, { t: "Blazor Deep Dive", p: "YouTube", d: "3h", pg: 30, tg: ["Blazor"], pr: "High" }], "Completed": [{ t: "Docker Fundamentals", p: "Udemy", d: "6h", tg: ["DevOps"], pr: "Med" }] };
  const dc = { "To Start": C.inkMuted, "In Progress": C.warning, "Completed": C.success };
  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 28px" }}>Video <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Learning</span></h1></Reveal>
    <Reveal delay={60}><div style={{ display: "grid", gridTemplateColumns: "repeat(4,1fr)", gap: 14, marginBottom: 24 }}>
      {[{ l: "Videos", v: "5" }, { l: "In Progress", v: "2" }, { l: "Hours", v: "23.5h" }, { l: "Streak", v: "3 days" }].map((m, i) =>
        <Card key={i}><div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", textTransform: "uppercase", letterSpacing: "0.12em", marginBottom: 6 }}>{m.l}</div>
          <div style={{ fontSize: 26, fontWeight: 700, color: C.ink, fontFamily: "'Libre Baskerville',serif" }}>{m.v}</div></Card>)}
    </div></Reveal>
    <Reveal delay={120}><div style={{ display: "grid", gridTemplateColumns: "repeat(3,1fr)", gap: 14 }}>
      {Object.entries(kb).map(([st, its], si) => <Card key={st}>
        <div style={{ display: "flex", alignItems: "center", gap: 8, marginBottom: 14 }}>
          <div style={{ width: 9, height: 9, borderRadius: "50%", background: dc[st] }} /><span style={{ fontSize: 11, color: C.inkSoft, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.08em" }}>{st}</span>
        </div>
        {its.map((it, i) => <div key={i} style={{ padding: 14, borderRadius: 12, background: C.pampas, marginBottom: 8, border: `1px solid ${C.border}` }}>
          <div style={{ fontSize: 13, fontWeight: 700, color: C.ink, fontFamily: "'Source Sans 3',sans-serif", marginBottom: 4 }}>{it.t}</div>
          <div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", marginBottom: 8 }}>{it.p} · {it.d}</div>
          {it.pg !== undefined && <div style={{ marginBottom: 8 }}>
            <div style={{ height: 5, borderRadius: 3, background: C.white, overflow: "hidden" }}><div style={{ height: "100%", width: `${it.pg}%`, borderRadius: 3, background: C.crail }} /></div>
            <span style={{ fontSize: 9, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace" }}>{it.pg}%</span></div>}
          <div style={{ display: "flex", gap: 4 }}>{it.tg.map(tg => <span key={tg} style={{ fontSize: 9, padding: "2px 8px", borderRadius: 6, background: C.crailPale, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 600 }}>{tg}</span>)}</div>
        </div>)}
      </Card>)}
    </div></Reveal>
  </div>);
}

// ═══════════════════════════════════════════
// SCREEN: BOOKS
// ═══════════════════════════════════════════
function BooksScreen() {
  const bk = [{ t: "Clean Architecture", a: "R.C. Martin", pg: 432, c: 432, st: "Completed", r: 5, cl: C.crail }, { t: "Data-Intensive Apps", a: "M. Kleppmann", pg: 616, c: 310, st: "In Progress", r: null, cl: "#6B8EC7" }, { t: "Atomic Habits", a: "J. Clear", pg: 320, c: 0, st: "To Start", r: null, cl: "#5B8C51" }, { t: "Pragmatic Programmer", a: "Hunt & Thomas", pg: 352, c: 200, st: "In Progress", r: null, cl: C.warning }];
  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 28px" }}>Book <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Collection</span></h1></Reveal>
    <Reveal delay={60}><div style={{ display: "grid", gridTemplateColumns: "repeat(4,1fr)", gap: 14, marginBottom: 24 }}>
      {[{ l: "Books", v: "4" }, { l: "Completed", v: "1" }, { l: "Pages", v: "942" }, { l: "Pace", v: "15/day" }].map((m, i) =>
        <Card key={i}><div style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace", textTransform: "uppercase", letterSpacing: "0.12em", marginBottom: 6 }}>{m.l}</div>
          <div style={{ fontSize: 26, fontWeight: 700, color: C.ink, fontFamily: "'Libre Baskerville',serif" }}>{m.v}</div></Card>)}
    </div></Reveal>
    {bk.map((b, i) => <Reveal key={i} delay={120 + i * 60}><Card style={{ marginBottom: 12, display: "flex", gap: 20, alignItems: "center" }}>
      <div style={{ width: 56, height: 76, borderRadius: 10, background: b.cl, display: "flex", alignItems: "center", justifyContent: "center", flexShrink: 0, boxShadow: `4px 6px 16px ${b.cl}20` }}>
        <span style={{ color: "#fff", fontSize: 26, fontWeight: 800, fontFamily: "'Libre Baskerville',serif" }}>{b.t[0]}</span>
      </div>
      <div style={{ flex: 1 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "start" }}>
          <div><div style={{ color: C.ink, fontSize: 17, fontWeight: 700, fontFamily: "'Libre Baskerville',serif" }}>{b.t}</div><div style={{ color: C.inkMuted, fontSize: 12, fontFamily: "'Source Sans 3',sans-serif" }}>{b.a}</div></div>
          <span style={{ fontSize: 10, padding: "4px 12px", borderRadius: 10, fontWeight: 700, fontFamily: "'JetBrains Mono',monospace", background: b.st === "Completed" ? C.successPale : b.st === "In Progress" ? C.warningPale : C.cloudyPale, color: b.st === "Completed" ? C.success : b.st === "In Progress" ? C.warning : C.inkMuted }}>{b.st}</span>
        </div>
        <div style={{ marginTop: 10 }}>
          <div style={{ height: 6, borderRadius: 3, background: C.pampas, overflow: "hidden" }}><div style={{ height: "100%", width: `${(b.c / b.pg) * 100}%`, borderRadius: 3, background: b.st === "Completed" ? C.success : C.crail, transition: "width 1s ease" }} /></div>
          <div style={{ display: "flex", justifyContent: "space-between", marginTop: 5 }}>
            <span style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace" }}>{b.c}/{b.pg} pages</span>
            {b.r && <span style={{ fontSize: 11, color: C.warning }}>{"★".repeat(b.r)}</span>}</div>
        </div>
      </div>
    </Card></Reveal>)}
  </div>);
}

// ═══════════════════════════════════════════
// SCREEN: PRODUCTIVITY
// ═══════════════════════════════════════════
function ProductivityScreen() {
  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 28px" }}>Productivity <span style={{ fontWeight: 700, fontStyle: "italic", color: C.crail }}>Hub</span></h1></Reveal>
    <Reveal delay={60}><Card accent style={{ marginBottom: 22, borderLeft: `4px solid ${C.crail}` }}>
      <div style={{ fontSize: 10, color: C.crail, fontFamily: "'JetBrains Mono',monospace", fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.15em", marginBottom: 12 }}>☀ Morning Briefing</div>
      <div style={{ fontSize: 15, color: C.ink, fontFamily: "'Libre Baskerville',serif", lineHeight: 1.8 }}>
        <p style={{ margin: "0 0 6px" }}>3 goals active: Sleep ≥7h ✓ · Exercise ≥1h ⏳ · Read 30min ⏳</p>
        <p style={{ margin: "0 0 6px" }}>Exercise streak: <strong>5 days</strong>. Yesterday: <strong>9h work</strong> (above avg).</p>
        <p style={{ margin: 0, color: C.crail, fontStyle: "italic" }}>"Consistency beats intensity. 28 days — remarkable."</p>
      </div>
    </Card></Reveal>
    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginBottom: 22 }}>
      <Reveal delay={120}><Card>
        <SectionLabel>Focus Score</SectionLabel>
        <div style={{ textAlign: "center", padding: "10px 0" }}>
          <svg width="150" height="150" viewBox="0 0 150 150">
            <circle cx="75" cy="75" r="62" fill="none" stroke={C.pampas} strokeWidth="10" />
            <circle cx="75" cy="75" r="62" fill="none" stroke={C.crail} strokeWidth="10" strokeDasharray={`${0.78 * 390} 390`} strokeLinecap="round" transform="rotate(-90 75 75)" style={{ transition: "stroke-dasharray 2s ease", filter: `drop-shadow(0 0 10px ${C.crail}30)` }} />
            <text x="75" y="70" textAnchor="middle" fill={C.ink} fontSize="40" fontWeight="700" fontFamily="'Libre Baskerville',serif">78</text>
            <text x="75" y="92" textAnchor="middle" fill={C.inkMuted} fontSize="12" fontFamily="'JetBrains Mono',monospace">/100</text>
          </svg>
        </div>
      </Card></Reveal>
      <Reveal delay={180}><Card>
        <SectionLabel>Planned vs Actual</SectionLabel>
        <ResponsiveContainer width="100%" height={155}>
          <BarChart data={[{ s: "Work", p: 8, a: 9 }, { s: "Exercise", p: 2, a: 1 }, { s: "Sleep", p: 8, a: 7 }, { s: "Social", p: 2, a: 3 }]}>
            <XAxis dataKey="s" tick={{ fill: C.inkMuted, fontSize: 10 }} axisLine={false} /><YAxis tick={{ fill: C.inkMuted, fontSize: 10 }} axisLine={false} />
            <Tooltip contentStyle={tt} /><Bar dataKey="p" fill={C.cloudyPale} radius={[6, 6, 0, 0]} name="Planned" /><Bar dataKey="a" fill={C.crail} radius={[6, 6, 0, 0]} name="Actual" />
          </BarChart>
        </ResponsiveContainer>
      </Card></Reveal>
    </div>
    <Reveal delay={240}><Card>
      <SectionLabel>Tasks</SectionLabel>
      {[{ t: "Review PR #342", pr: "High", tm: "09:00", dn: true }, { t: "Update docs", pr: "Med", tm: "11:00", dn: false }, { t: "Gym — legs", pr: "High", tm: "17:00", dn: false }, { t: "Read ch.5", pr: "Low", tm: "21:00", dn: false }].map((tk, i) =>
        <div key={i} style={{ display: "flex", alignItems: "center", gap: 14, padding: "12px 0", borderBottom: i < 3 ? `1px solid ${C.border}` : "none" }}>
          <div style={{ width: 22, height: 22, borderRadius: 7, border: tk.dn ? "none" : `2px solid ${C.border}`, background: tk.dn ? C.success : "transparent", display: "flex", alignItems: "center", justifyContent: "center" }}>{tk.dn && <span style={{ color: "#fff", fontSize: 11, fontWeight: 800 }}>✓</span>}</div>
          <span style={{ flex: 1, color: tk.dn ? C.inkMuted : C.ink, fontSize: 14, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 600, textDecoration: tk.dn ? "line-through" : "none" }}>{tk.t}</span>
          <span style={{ fontSize: 10, color: C.inkMuted, fontFamily: "'JetBrains Mono',monospace" }}>{tk.tm}</span>
          <span style={{ fontSize: 9, padding: "3px 10px", borderRadius: 8, fontWeight: 700, fontFamily: "'JetBrains Mono',monospace", background: tk.pr === "High" ? C.dangerPale : tk.pr === "Med" ? C.warningPale : C.cloudyPale, color: tk.pr === "High" ? C.danger : tk.pr === "Med" ? C.warning : C.inkMuted }}>{tk.pr}</span>
        </div>)}
    </Card></Reveal>
  </div>);
}

// ═══════════════════════════════════════════
// SCREEN: SETTINGS
// ═══════════════════════════════════════════
function SettingsScreen() {
  const secs = [
    { s: "Tracking", it: [{ l: "Time Granularity", v: "1 Hour" }, { l: "Period", v: "Jan 1 – Dec 31, 2026" }, { l: "Monthly Budget", v: "$500" }] },
    { s: "AI Features", it: [{ l: "Auto-Fill Logger", v: true, ty: "toggle" }, { l: "Smart Suggestions", v: true, ty: "toggle" }, { l: "Daily Summary", v: false, ty: "toggle" }, { l: "Goal Coaching", v: true, ty: "toggle" }, { l: "Confidence", v: "70%" }] },
    { s: "Notifications", it: [{ l: "Interval", v: "Every 3 hours" }, { l: "Quiet Hours", v: "11 PM – 7 AM" }, { l: "Push", v: true, ty: "toggle" }] },
    { s: "Data", it: [{ l: "Export", v: "Excel / CSV / PDF", ty: "btn" }, { l: "Import", v: "Upload .xlsx", ty: "btn" }] },
  ];
  return (<div>
    <Reveal><h1 style={{ fontSize: 42, fontWeight: 300, color: C.ink, fontFamily: "'Libre Baskerville',serif", margin: "0 0 28px" }}>Settings</h1></Reveal>
    {secs.map((sec, si) => <Reveal key={si} delay={si * 50}><div style={{ marginBottom: 24 }}>
      <SectionLabel>{sec.s}</SectionLabel>
      <Card style={{ padding: 0, overflow: "hidden" }}>
        {sec.it.map((it, i) => <div key={i} style={{ display: "flex", alignItems: "center", justifyContent: "space-between", padding: "16px 22px", borderBottom: i < sec.it.length - 1 ? `1px solid ${C.border}` : "none" }}>
          <span style={{ color: C.ink, fontSize: 14, fontFamily: "'Source Sans 3',sans-serif", fontWeight: 600 }}>{it.l}</span>
          {it.ty === "toggle" ? <div style={{ width: 48, height: 26, borderRadius: 13, background: it.v ? C.crail : C.pampas, cursor: "pointer", position: "relative", transition: "all 0.3s", boxShadow: it.v ? `0 0 12px ${C.crail}25` : "none" }}>
            <div style={{ width: 20, height: 20, borderRadius: "50%", background: C.white, position: "absolute", top: 3, left: it.v ? 25 : 3, transition: "left 0.3s cubic-bezier(0.34,1.56,0.64,1)", boxShadow: "0 2px 6px rgba(0,0,0,0.1)" }} /></div>
            : it.ty === "btn" ? <button style={{ padding: "7px 16px", borderRadius: 10, background: C.crailPale, border: `1px solid ${C.borderAccent}`, color: C.crail, fontSize: 12, cursor: "pointer", fontFamily: "'JetBrains Mono',monospace", fontWeight: 600 }}>{it.v}</button>
              : <span style={{ color: C.inkSoft, fontSize: 13, fontFamily: "'JetBrains Mono',monospace" }}>{it.v}</span>}
        </div>)}
      </Card>
    </div></Reveal>)}
  </div>);
}

// ═══════════════════════════════════════════
// MAIN APP SHELL
// ═══════════════════════════════════════════
export default function App() {
  const [screen, setScreen] = useState("daily");
  const nav = [
    { id: "daily", l: "Logger", i: Ic.grid }, { id: "dashboard", l: "Dashboard", i: Ic.bar }, { id: "spending", l: "Spending", i: Ic.dollar },
    { id: "goals", l: "Goals", i: Ic.target }, { id: "pulse", l: "Life Pulse", i: Ic.pulse }, { id: "habits", l: "Habits", i: Ic.check },
    { id: "review", l: "Review", i: Ic.edit }, { id: "productivity", l: "Productivity", i: Ic.zap }, { id: "videos", l: "Videos", i: Ic.play },
    { id: "books", l: "Books", i: Ic.book }, { id: "settings", l: "Settings", i: Ic.gear },
  ];
  const sc = { daily: <DailyLogScreen />, dashboard: <DashboardScreen />, spending: <SpendingScreen />, goals: <GoalsScreen />, pulse: <LifePulseScreen />, habits: <HabitScreen />, review: <WeeklyReviewScreen />, productivity: <ProductivityScreen />, videos: <VideosScreen />, books: <BooksScreen />, settings: <SettingsScreen /> };

  return (
    <div style={{ minHeight: "100vh", background: C.pampas, display: "flex" }}>
      <style>{`
        @import url('https://fonts.googleapis.com/css2?family=Libre+Baskerville:ital,wght@0,400;0,700;1,400&family=Source+Sans+3:wght@400;500;600;700&family=JetBrains+Mono:wght@400;500;600;700&display=swap');
        *{box-sizing:border-box;margin:0;padding:0}
        ::-webkit-scrollbar{width:5px}::-webkit-scrollbar-track{background:transparent}::-webkit-scrollbar-thumb{background:${C.cloudy}40;border-radius:3px}
        @keyframes shimmer{0%{transform:translateX(-100%)}100%{transform:translateX(100%)}}
        @keyframes fadeIn{from{opacity:0;transform:translateY(12px)}to{opacity:1;transform:translateY(0)}}
      `}</style>

      {/* ── Sidebar ── */}
      <nav style={{ width: 232, flexShrink: 0, background: C.white, borderRight: `1px solid ${C.border}`, padding: "28px 14px", display: "flex", flexDirection: "column", position: "relative" }}>
        {/* Decorative top accent */}
        <div style={{ position: "absolute", top: 0, left: 0, right: 0, height: 3, background: `linear-gradient(90deg,${C.crail},${C.crailLight},${C.cloudy})` }} />

        <div style={{ display: "flex", alignItems: "center", gap: 12, padding: "0 10px", marginBottom: 32 }}>
          <div style={{ width: 42, height: 42, borderRadius: 12, background: C.crail, display: "flex", alignItems: "center", justifyContent: "center", boxShadow: `0 4px 16px ${C.crail}30` }}>
            <span style={{ color: C.white, fontSize: 22, fontWeight: 700, fontFamily: "'Libre Baskerville',serif" }}>L</span>
          </div>
          <div>
            <div style={{ color: C.ink, fontSize: 18, fontWeight: 700, fontFamily: "'Libre Baskerville',serif", lineHeight: 1 }}>LifeTracker</div>
            <div style={{ fontSize: 9, fontWeight: 700, fontFamily: "'JetBrains Mono',monospace", color: C.crail, letterSpacing: "0.2em", marginTop: 2 }}>PRO</div>
          </div>
        </div>

        <div style={{ display: "flex", flexDirection: "column", gap: 2 }}>
          {nav.map(n => <button key={n.id} onClick={() => setScreen(n.id)} style={{
            display: "flex", alignItems: "center", gap: 11, padding: "10px 14px", borderRadius: 10, border: "none", width: "100%", textAlign: "left", cursor: "pointer",
            transition: "all 0.25s", fontFamily: "'Source Sans 3',sans-serif", fontSize: 13.5,
            background: screen === n.id ? C.crailPale : "transparent",
            color: screen === n.id ? C.crail : C.inkMuted, fontWeight: screen === n.id ? 700 : 500,
            borderLeft: screen === n.id ? `3px solid ${C.crail}` : "3px solid transparent",
          }}>{n.i}{n.l}</button>)}
        </div>

        <div style={{ flex: 1 }} />
        <div style={{ padding: "14px", borderRadius: 14, background: C.pampas, border: `1px solid ${C.border}` }}>
          <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
            <div style={{ width: 34, height: 34, borderRadius: 10, background: C.crailPale, display: "flex", alignItems: "center", justifyContent: "center", color: C.crail, fontSize: 15, fontWeight: 800, fontFamily: "'Libre Baskerville',serif" }}>A</div>
            <div><div style={{ color: C.ink, fontSize: 13, fontWeight: 700, fontFamily: "'Source Sans 3',sans-serif" }}>Akshay</div>
              <div style={{ color: C.inkMuted, fontSize: 10, fontFamily: "'JetBrains Mono',monospace" }}>28-day streak 🔥</div></div>
          </div>
        </div>
      </nav>

      {/* ── Main ── */}
      <main key={screen} style={{ flex: 1, padding: "32px 36px", overflowY: "auto", maxHeight: "100vh", animation: "fadeIn 0.35s ease-out" }}>
        {sc[screen]}
      </main>
    </div>
  );
}