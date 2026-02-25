import { useState, useEffect, useRef } from "react";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, LineChart, Line, CartesianGrid, Legend, AreaChart, Area } from "recharts";

// ─── Data from actual Excel ───
const CATEGORIES = [
  { code: 0, name: "Sleep", letter: "S", color: "#6366f1" },
  { code: 1, name: "Work", letter: "W", color: "#ef4444" },
  { code: 2, name: "Hobbies / Projects", letter: "H", color: "#d4a373" },
  { code: 3, name: "Social / Mobile", letter: "S", color: "#eab308" },
  { code: 4, name: "Exercise", letter: "E", color: "#16a34a" },
  { code: 5, name: "Friends", letter: "F", color: "#06b6d4" },
  { code: 6, name: "Relaxation / Book", letter: "B", color: "#2563eb" },
  { code: 7, name: "Partner", letter: "P", color: "#db2777" },
  { code: 8, name: "Family", letter: "F", color: "#7c3aed" },
  { code: 9, name: "Productive / Chores", letter: "P", color: "#65a30d" },
  { code: 10, name: "Travel", letter: "T", color: "#14b8a6" },
  { code: 11, name: "Misc / Getting Ready", letter: "M", color: "#f97316" },
];

const DASHBOARD_DATA = [
  { name: "Sleep", hours: 246, pct: 37.0 },
  { name: "Work", hours: 131, pct: 19.7 },
  { name: "Social", hours: 88, pct: 13.3 },
  { name: "Exercise", hours: 37, pct: 5.6 },
  { name: "Book", hours: 17, pct: 2.6 },
  { name: "Productive", hours: 34, pct: 5.1 },
  { name: "Friends", hours: 61, pct: 9.2 },
  { name: "Family", hours: 35, pct: 5.3 },
  { name: "Travel", hours: 4, pct: 0.6 },
  { name: "Getting Ready", hours: 11, pct: 1.7 },
];

const MONTHLY_SPEND = [
  { month: "JAN", amount: 93.5, rent: 812 },
  { month: "FEB", amount: 280, rent: 812 },
  { month: "MAR", amount: 0, rent: 0 },
  { month: "APR", amount: 0, rent: 0 },
];

const SAMPLE_DAYS = [
  { date: "Jan 25", day: "Sat", hours: ["M","M","S","S","S","S","S","S","W","E","E","S","W","W","W","E","W","H","F","F","F","F","W","B"], weight: 91.2, spent: 0, highlight: "Support office work, day one of fasting" },
  { date: "Jan 26", day: "Sun", hours: ["M","M","M","S","S","S","S","S","S","S","S","S","S","S","S","E","M","M","S","S","S","S","M","M"], weight: 90.8, spent: 24.5, highlight: "Not good day" },
  { date: "Jan 27", day: "Mon", hours: ["M","M","M","M","S","S","S","S","S","G","W","W","W","W","H","S","S","S","S","E","M","M","M","M"], weight: 90.0, spent: 17, highlight: "Narender came, Not good day" },
  { date: "Jan 28", day: "Tue", hours: ["M","M","S","S","S","S","S","S","S","G","W","W","W","W","W","W","W","H","H","E","E","M","M","S"], weight: 89.5, spent: 0, highlight: "Good productive work day" },
  { date: "Jan 29", day: "Wed", hours: ["M","M","S","S","S","S","S","S","S","G","W","W","W","W","W","W","W","S","E","E","H","H","M","M"], weight: 89.2, spent: 12, highlight: "Solid work session, evening hobbies" },
  { date: "Jan 30", day: "Thu", hours: ["M","M","M","M","S","S","S","S","S","G","W","W","W","W","W","E","S","S","S","F","F","F","M","M"], weight: 89.0, spent: 35, highlight: "Met friends in evening" },
  { date: "Jan 31", day: "Fri", hours: ["M","M","S","S","S","S","S","S","S","G","W","W","W","W","H","H","E","E","S","S","B","B","M","M"], weight: 88.8, spent: 8, highlight: "End of month, reading time" },
];

const WEEKLY_TRENDS = [
  { week: "W1", sleep: 56, work: 32, exercise: 8, social: 18 },
  { week: "W2", sleep: 54, work: 38, exercise: 10, social: 14 },
  { week: "W3", sleep: 58, work: 35, exercise: 7, social: 16 },
  { week: "W4", sleep: 52, work: 40, exercise: 12, social: 12 },
];

const letterToCategory = (l) => CATEGORIES.find(c => c.letter === l) || CATEGORIES[11];
const letterToColor = (l) => {
  const map = { S: "#6366f1", W: "#ef4444", H: "#d4a373", E: "#16a34a", F: "#06b6d4", B: "#2563eb", P: "#db2777", G: "#f97316", M: "#f97316", T: "#14b8a6" };
  return map[l] || "#64748b";
};

// ─── Navigation Icons (inline SVGs) ───
const Icons = {
  home: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z"/><polyline points="9,22 9,12 15,12 15,22"/></svg>,
  grid: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/></svg>,
  chart: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><line x1="18" y1="20" x2="18" y2="10"/><line x1="12" y1="20" x2="12" y2="4"/><line x1="6" y1="20" x2="6" y2="14"/></svg>,
  dollar: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 000 7h5a3.5 3.5 0 010 7H6"/></svg>,
  target: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><circle cx="12" cy="12" r="10"/><circle cx="12" cy="12" r="6"/><circle cx="12" cy="12" r="2"/></svg>,
  book: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M4 19.5A2.5 2.5 0 016.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z"/></svg>,
  play: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><polygon points="5,3 19,12 5,21"/></svg>,
  zap: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><polygon points="13,2 3,14 12,14 11,22 21,10 12,10"/></svg>,
  settings: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.65 1.65 0 00.33 1.82l.06.06a2 2 0 010 2.83 2 2 0 01-2.83 0l-.06-.06a1.65 1.65 0 00-1.82-.33 1.65 1.65 0 00-1 1.51V21a2 2 0 01-4 0v-.09A1.65 1.65 0 009 19.4a1.65 1.65 0 00-1.82.33l-.06.06a2 2 0 01-2.83-2.83l.06-.06A1.65 1.65 0 004.68 15a1.65 1.65 0 00-1.51-1H3a2 2 0 010-4h.09A1.65 1.65 0 004.6 9a1.65 1.65 0 00-.33-1.82l-.06-.06a2 2 0 012.83-2.83l.06.06A1.65 1.65 0 009 4.68a1.65 1.65 0 001-1.51V3a2 2 0 014 0v.09a1.65 1.65 0 001 1.51 1.65 1.65 0 001.82-.33l.06-.06a2 2 0 012.83 2.83l-.06.06A1.65 1.65 0 0019.4 9a1.65 1.65 0 001.51 1H21a2 2 0 010 4h-.09a1.65 1.65 0 00-1.51 1z"/></svg>,
  camera: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M23 19a2 2 0 01-2 2H3a2 2 0 01-2-2V8a2 2 0 012-2h4l2-3h6l2 3h4a2 2 0 012 2z"/><circle cx="12" cy="13" r="4"/></svg>,
  star: <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" strokeWidth="1"><polygon points="12,2 15.09,8.26 22,9.27 17,14.14 18.18,21.02 12,17.77 5.82,21.02 7,14.14 2,9.27 8.91,8.26"/></svg>,
  sparkle: <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor"><path d="M12 0L14.59 8.41L23 12L14.59 15.59L12 24L9.41 15.59L1 12L9.41 8.41Z"/></svg>,
  check: <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3"><polyline points="20,6 9,17 4,12"/></svg>,
  arrow: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><line x1="5" y1="12" x2="19" y2="12"/><polyline points="12,5 19,12 12,19"/></svg>,
  user: <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>,
};

// ─── Screen Components ───

function LoginScreen({ onLogin }) {
  const [isRegister, setIsRegister] = useState(false);
  return (
    <div style={{ minHeight: "100vh", display: "flex", alignItems: "center", justifyContent: "center", background: "linear-gradient(160deg, #0c0a1a 0%, #1a1333 40%, #0f172a 100%)", padding: 20 }}>
      <div style={{ position: "absolute", inset: 0, overflow: "hidden", pointerEvents: "none" }}>
        {[...Array(6)].map((_, i) => (
          <div key={i} style={{ position: "absolute", width: 300 + i * 80, height: 300 + i * 80, borderRadius: "50%", background: `radial-gradient(circle, ${["rgba(99,102,241,0.08)","rgba(219,39,119,0.06)","rgba(6,182,212,0.07)","rgba(22,163,74,0.05)","rgba(234,179,8,0.06)","rgba(239,68,68,0.05)"][i]} 0%, transparent 70%)`, top: `${[10,60,30,70,20,80][i]}%`, left: `${[70,10,50,80,20,60][i]}%`, transform: "translate(-50%,-50%)", animation: `float${i} ${20+i*5}s ease-in-out infinite` }} />
        ))}
      </div>
      <div style={{ width: "100%", maxWidth: 420, position: "relative", zIndex: 1 }}>
        <div style={{ textAlign: "center", marginBottom: 36 }}>
          <div style={{ display: "inline-flex", alignItems: "center", gap: 10, marginBottom: 8 }}>
            <div style={{ width: 44, height: 44, borderRadius: 12, background: "linear-gradient(135deg, #6366f1, #8b5cf6)", display: "flex", alignItems: "center", justifyContent: "center" }}>
              <span style={{ color: "#fff", fontSize: 22, fontWeight: 800 }}>L</span>
            </div>
            <span style={{ fontFamily: "'Playfair Display', Georgia, serif", fontSize: 28, fontWeight: 700, color: "#f8fafc", letterSpacing: "-0.02em" }}>LifeTracker</span>
            <span style={{ fontFamily: "'Space Mono', monospace", fontSize: 11, color: "#818cf8", background: "rgba(99,102,241,0.15)", padding: "2px 8px", borderRadius: 6, fontWeight: 600 }}>PRO</span>
          </div>
          <p style={{ color: "#94a3b8", fontSize: 14, fontFamily: "'DM Sans', sans-serif", margin: 0, marginTop: 4 }}>Track every hour. Master your life.</p>
        </div>
        <div style={{ background: "rgba(15,23,42,0.8)", backdropFilter: "blur(20px)", border: "1px solid rgba(99,102,241,0.15)", borderRadius: 20, padding: 32, boxShadow: "0 25px 50px rgba(0,0,0,0.4)" }}>
          <h2 style={{ color: "#f1f5f9", fontSize: 20, fontWeight: 600, margin: "0 0 24px", fontFamily: "'DM Sans', sans-serif" }}>{isRegister ? "Create Account" : "Welcome Back"}</h2>
          {isRegister && <input placeholder="Full Name" style={inputStyle} />}
          <input placeholder="Email" style={inputStyle} />
          <input placeholder="Password" type="password" style={inputStyle} />
          {isRegister && <input placeholder="Confirm Password" type="password" style={inputStyle} />}
          <button onClick={onLogin} style={{ width: "100%", padding: "14px 20px", background: "linear-gradient(135deg, #6366f1, #8b5cf6)", color: "#fff", border: "none", borderRadius: 12, fontSize: 15, fontWeight: 600, cursor: "pointer", fontFamily: "'DM Sans', sans-serif", marginTop: 8, transition: "all 0.2s", boxShadow: "0 4px 15px rgba(99,102,241,0.3)" }}>
            {isRegister ? "Create Account" : "Sign In"}
          </button>
          <div style={{ display: "flex", alignItems: "center", gap: 12, margin: "20px 0" }}>
            <div style={{ flex: 1, height: 1, background: "rgba(148,163,184,0.2)" }} />
            <span style={{ color: "#64748b", fontSize: 12, fontFamily: "'DM Sans', sans-serif" }}>or continue with</span>
            <div style={{ flex: 1, height: 1, background: "rgba(148,163,184,0.2)" }} />
          </div>
          <div style={{ display: "flex", gap: 12 }}>
            {["Google", "Apple"].map(p => (
              <button key={p} onClick={onLogin} style={{ flex: 1, padding: "12px", background: "rgba(30,41,59,0.8)", border: "1px solid rgba(148,163,184,0.15)", borderRadius: 10, color: "#e2e8f0", fontSize: 13, fontWeight: 500, cursor: "pointer", fontFamily: "'DM Sans', sans-serif" }}>{p}</button>
            ))}
          </div>
          <p style={{ textAlign: "center", marginTop: 20, color: "#64748b", fontSize: 13, fontFamily: "'DM Sans', sans-serif" }}>
            {isRegister ? "Already have an account? " : "Don't have an account? "}
            <span onClick={() => setIsRegister(!isRegister)} style={{ color: "#818cf8", cursor: "pointer", fontWeight: 600 }}>{isRegister ? "Sign In" : "Register"}</span>
          </p>
        </div>
      </div>
    </div>
  );
}

const inputStyle = { width: "100%", padding: "12px 16px", background: "rgba(30,41,59,0.6)", border: "1px solid rgba(148,163,184,0.15)", borderRadius: 10, color: "#e2e8f0", fontSize: 14, fontFamily: "'DM Sans', sans-serif", marginBottom: 12, outline: "none", boxSizing: "border-box" };

function DailyLogScreen() {
  const [selectedDay, setSelectedDay] = useState(0);
  const [editCell, setEditCell] = useState(null);
  const [showAutoFill, setShowAutoFill] = useState(false);
  const [autoFilled, setAutoFilled] = useState(false);
  const day = SAMPLE_DAYS[selectedDay];

  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 20, flexWrap: "wrap", gap: 12 }}>
        <div>
          <h2 style={h2Style}>Daily Time Logger</h2>
          <p style={subtitleStyle}>Tap any cell to change category</p>
        </div>
        <div style={{ display: "flex", gap: 8 }}>
          <button onClick={() => { setShowAutoFill(true); setTimeout(() => { setAutoFilled(true); setShowAutoFill(false); }, 1500); }} style={{ ...btnStyle, background: autoFilled ? "rgba(22,163,74,0.2)" : "linear-gradient(135deg, #6366f1, #8b5cf6)", color: autoFilled ? "#4ade80" : "#fff", border: autoFilled ? "1px solid rgba(74,222,128,0.3)" : "none" }}>
            {showAutoFill ? <span style={{ display: "flex", alignItems: "center", gap: 6 }}><span className="spin">⟳</span> Analyzing...</span> : autoFilled ? <span style={{ display: "flex", alignItems: "center", gap: 6 }}>{Icons.check} Auto-Filled</span> : <span style={{ display: "flex", alignItems: "center", gap: 6 }}>{Icons.sparkle} Auto-Fill Day</span>}
          </button>
        </div>
      </div>

      {/* Day selector */}
      <div style={{ display: "flex", gap: 6, marginBottom: 16, overflowX: "auto", paddingBottom: 4 }}>
        {SAMPLE_DAYS.map((d, i) => (
          <button key={i} onClick={() => setSelectedDay(i)} style={{ padding: "8px 14px", borderRadius: 10, border: i === selectedDay ? "1px solid #6366f1" : "1px solid rgba(148,163,184,0.1)", background: i === selectedDay ? "rgba(99,102,241,0.15)" : "rgba(15,23,42,0.4)", color: i === selectedDay ? "#a5b4fc" : "#94a3b8", fontSize: 12, fontFamily: "'DM Sans', sans-serif", cursor: "pointer", whiteSpace: "nowrap", fontWeight: i === selectedDay ? 600 : 400 }}>
            <div>{d.day}</div>
            <div style={{ fontSize: 11, opacity: 0.7 }}>{d.date}</div>
          </button>
        ))}
      </div>

      {/* Time grid */}
      <div style={{ background: "rgba(15,23,42,0.5)", borderRadius: 16, border: "1px solid rgba(148,163,184,0.08)", padding: 16, marginBottom: 16 }}>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(12, 1fr)", gap: 4, marginBottom: 4 }}>
          {[...Array(12)].map((_, i) => (
            <div key={i} style={{ textAlign: "center", fontSize: 10, color: "#64748b", fontFamily: "'Space Mono', monospace", padding: "2px 0" }}>{i}:00</div>
          ))}
        </div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(12, 1fr)", gap: 4, marginBottom: 8 }}>
          {day.hours.slice(0, 12).map((h, i) => (
            <div key={i} onClick={() => setEditCell(editCell === i ? null : i)} style={{ height: 40, borderRadius: 8, background: letterToColor(h), display: "flex", alignItems: "center", justifyContent: "center", cursor: "pointer", fontSize: 13, fontWeight: 700, color: "#fff", fontFamily: "'Space Mono', monospace", transition: "all 0.15s", transform: editCell === i ? "scale(1.15)" : "scale(1)", boxShadow: editCell === i ? `0 0 20px ${letterToColor(h)}40` : "none", position: "relative", zIndex: editCell === i ? 2 : 1 }}>
              {h}
              {autoFilled && i > 0 && <div style={{ position: "absolute", top: -2, right: -2, width: 8, height: 8, borderRadius: "50%", background: i % 3 === 0 ? "#eab308" : "#22c55e", border: "1px solid rgba(0,0,0,0.3)" }} />}
            </div>
          ))}
        </div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(12, 1fr)", gap: 4, marginBottom: 4 }}>
          {[...Array(12)].map((_, i) => (
            <div key={i} style={{ textAlign: "center", fontSize: 10, color: "#64748b", fontFamily: "'Space Mono', monospace", padding: "2px 0" }}>{i+12}:00</div>
          ))}
        </div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(12, 1fr)", gap: 4 }}>
          {day.hours.slice(12, 24).map((h, i) => (
            <div key={i+12} onClick={() => setEditCell(editCell === i+12 ? null : i+12)} style={{ height: 40, borderRadius: 8, background: letterToColor(h), display: "flex", alignItems: "center", justifyContent: "center", cursor: "pointer", fontSize: 13, fontWeight: 700, color: "#fff", fontFamily: "'Space Mono', monospace", transition: "all 0.15s", transform: editCell === i+12 ? "scale(1.15)" : "scale(1)", boxShadow: editCell === i+12 ? `0 0 20px ${letterToColor(h)}40` : "none", position: "relative", zIndex: editCell === i+12 ? 2 : 1 }}>
              {h}
              {autoFilled && (i+12) % 4 === 0 && <div style={{ position: "absolute", top: -2, right: -2, width: 8, height: 8, borderRadius: "50%", background: "#eab308", border: "1px solid rgba(0,0,0,0.3)" }} />}
            </div>
          ))}
        </div>
      </div>

      {/* Category legend */}
      {editCell !== null && (
        <div style={{ background: "rgba(15,23,42,0.7)", borderRadius: 14, border: "1px solid rgba(99,102,241,0.2)", padding: 14, marginBottom: 16 }}>
          <p style={{ color: "#94a3b8", fontSize: 12, margin: "0 0 10px", fontFamily: "'DM Sans', sans-serif" }}>Select category for hour {editCell}:00</p>
          <div style={{ display: "flex", flexWrap: "wrap", gap: 6 }}>
            {CATEGORIES.map(c => (
              <button key={c.code} onClick={() => setEditCell(null)} style={{ padding: "6px 12px", borderRadius: 8, background: c.color + "20", border: `1px solid ${c.color}40`, color: c.color, fontSize: 12, cursor: "pointer", fontFamily: "'DM Sans', sans-serif", fontWeight: 500 }}>
                <span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, marginRight: 4 }}>{c.letter}</span>{c.name}
              </button>
            ))}
          </div>
        </div>
      )}

      {/* Day metadata */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 12, marginBottom: 16 }}>
        {[
          { label: "Weight", value: `${day.weight} kg`, icon: "⚖️" },
          { label: "Spent", value: `$${day.spent}`, icon: "💵" },
          { label: "Hours Logged", value: "24/24", icon: "✓" },
        ].map((m, i) => (
          <div key={i} style={metricCardStyle}>
            <span style={{ fontSize: 18 }}>{m.icon}</span>
            <div style={{ fontSize: 18, fontWeight: 700, color: "#f1f5f9", fontFamily: "'DM Sans', sans-serif" }}>{m.value}</div>
            <div style={{ fontSize: 11, color: "#64748b", fontFamily: "'DM Sans', sans-serif" }}>{m.label}</div>
          </div>
        ))}
      </div>

      {/* Highlight */}
      <div style={{ background: "rgba(99,102,241,0.08)", borderRadius: 14, border: "1px solid rgba(99,102,241,0.15)", padding: 16 }}>
        <div style={{ fontSize: 11, color: "#818cf8", fontWeight: 600, marginBottom: 6, fontFamily: "'DM Sans', sans-serif", textTransform: "uppercase", letterSpacing: "0.05em" }}>✨ Highlight of the Day</div>
        <div style={{ color: "#e2e8f0", fontSize: 15, fontFamily: "'DM Sans', sans-serif", lineHeight: 1.5 }}>{day.highlight}</div>
      </div>
    </div>
  );
}

function DashboardScreen() {
  const colors = CATEGORIES.map(c => c.color);
  return (
    <div>
      <h2 style={h2Style}>Dashboard</h2>
      <p style={subtitleStyle}>Analytics & insights from your daily logs</p>

      {/* KPI Row */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12, marginBottom: 24 }}>
        {[
          { label: "Days Logged", value: "28", delta: "+4 this week", color: "#6366f1" },
          { label: "Total Hours", value: "664", delta: "100% logged", color: "#22c55e" },
          { label: "Avg Sleep", value: "8.8h", delta: "▲ 0.3h", color: "#818cf8" },
          { label: "Total Spent", value: "$373", delta: "Under budget", color: "#eab308" },
        ].map((k, i) => (
          <div key={i} style={{ ...metricCardStyle, borderLeft: `3px solid ${k.color}` }}>
            <div style={{ fontSize: 24, fontWeight: 800, color: "#f1f5f9", fontFamily: "'DM Sans', sans-serif" }}>{k.value}</div>
            <div style={{ fontSize: 12, color: "#94a3b8", fontFamily: "'DM Sans', sans-serif" }}>{k.label}</div>
            <div style={{ fontSize: 10, color: k.color, fontWeight: 600, marginTop: 4, fontFamily: "'DM Sans', sans-serif" }}>{k.delta}</div>
          </div>
        ))}
      </div>

      {/* Charts row */}
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginBottom: 24 }}>
        <div style={chartCardStyle}>
          <h3 style={chartTitleStyle}>Time Distribution</h3>
          <ResponsiveContainer width="100%" height={220}>
            <PieChart>
              <Pie data={DASHBOARD_DATA} cx="50%" cy="50%" innerRadius={50} outerRadius={85} dataKey="hours" stroke="none">
                {DASHBOARD_DATA.map((_, i) => <Cell key={i} fill={colors[i] || "#64748b"} />)}
              </Pie>
              <Tooltip contentStyle={tooltipStyle} formatter={(v) => `${v}h`} />
            </PieChart>
          </ResponsiveContainer>
        </div>
        <div style={chartCardStyle}>
          <h3 style={chartTitleStyle}>Hours by Category</h3>
          <ResponsiveContainer width="100%" height={220}>
            <BarChart data={DASHBOARD_DATA} layout="vertical" margin={{ left: 60 }}>
              <XAxis type="number" tick={{ fill: "#64748b", fontSize: 10 }} />
              <YAxis type="category" dataKey="name" tick={{ fill: "#94a3b8", fontSize: 11 }} width={58} />
              <Tooltip contentStyle={tooltipStyle} />
              <Bar dataKey="hours" radius={[0, 6, 6, 0]}>
                {DASHBOARD_DATA.map((_, i) => <Cell key={i} fill={colors[i] || "#64748b"} />)}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Weekly trends */}
      <div style={chartCardStyle}>
        <h3 style={chartTitleStyle}>Weekly Trends</h3>
        <ResponsiveContainer width="100%" height={220}>
          <AreaChart data={WEEKLY_TRENDS}>
            <CartesianGrid strokeDasharray="3 3" stroke="rgba(148,163,184,0.08)" />
            <XAxis dataKey="week" tick={{ fill: "#64748b", fontSize: 11 }} />
            <YAxis tick={{ fill: "#64748b", fontSize: 11 }} />
            <Tooltip contentStyle={tooltipStyle} />
            <Area type="monotone" dataKey="sleep" stackId="1" stroke="#6366f1" fill="#6366f1" fillOpacity={0.3} />
            <Area type="monotone" dataKey="work" stackId="1" stroke="#ef4444" fill="#ef4444" fillOpacity={0.3} />
            <Area type="monotone" dataKey="exercise" stackId="1" stroke="#16a34a" fill="#16a34a" fillOpacity={0.3} />
            <Area type="monotone" dataKey="social" stackId="1" stroke="#eab308" fill="#eab308" fillOpacity={0.3} />
            <Legend wrapperStyle={{ fontSize: 11, color: "#94a3b8" }} />
          </AreaChart>
        </ResponsiveContainer>
      </div>

      {/* Category legend */}
      <div style={{ display: "flex", flexWrap: "wrap", gap: 8, marginTop: 16 }}>
        {CATEGORIES.map(c => (
          <div key={c.code} style={{ display: "flex", alignItems: "center", gap: 6, padding: "4px 10px", borderRadius: 8, background: "rgba(15,23,42,0.5)" }}>
            <div style={{ width: 10, height: 10, borderRadius: 3, background: c.color }} />
            <span style={{ fontSize: 11, color: "#94a3b8", fontFamily: "'DM Sans', sans-serif" }}>{c.name}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

function SpendingScreen() {
  return (
    <div>
      <h2 style={h2Style}>Spending & Budget</h2>
      <p style={subtitleStyle}>Track expenses, set budgets, manage investments</p>

      {/* Budget overview */}
      <div style={{ ...chartCardStyle, marginBottom: 16 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
          <h3 style={{ ...chartTitleStyle, margin: 0 }}>February Budget</h3>
          <span style={{ color: "#eab308", fontSize: 13, fontWeight: 600, fontFamily: "'DM Sans', sans-serif" }}>$280 / $500</span>
        </div>
        <div style={{ height: 12, borderRadius: 6, background: "rgba(148,163,184,0.1)", overflow: "hidden" }}>
          <div style={{ height: "100%", width: "56%", borderRadius: 6, background: "linear-gradient(90deg, #eab308, #f59e0b)", transition: "width 1s ease" }} />
        </div>
        <div style={{ display: "flex", justifyContent: "space-between", marginTop: 6 }}>
          <span style={{ fontSize: 11, color: "#64748b", fontFamily: "'DM Sans', sans-serif" }}>56% used</span>
          <span style={{ fontSize: 11, color: "#22c55e", fontFamily: "'DM Sans', sans-serif" }}>$220 remaining</span>
        </div>
      </div>

      {/* Spend metrics */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 12, marginBottom: 20 }}>
        {[
          { label: "Avg / Month", value: "$31.13", color: "#6366f1" },
          { label: "Highest Month", value: "FEB: $280", color: "#ef4444" },
          { label: "Total Year", value: "$373.50", color: "#22c55e" },
        ].map((m, i) => (
          <div key={i} style={metricCardStyle}>
            <div style={{ fontSize: 18, fontWeight: 700, color: m.color, fontFamily: "'DM Sans', sans-serif" }}>{m.value}</div>
            <div style={{ fontSize: 11, color: "#64748b", fontFamily: "'DM Sans', sans-serif" }}>{m.label}</div>
          </div>
        ))}
      </div>

      {/* Monthly chart */}
      <div style={{ ...chartCardStyle, marginBottom: 16 }}>
        <h3 style={chartTitleStyle}>Monthly Spending</h3>
        <ResponsiveContainer width="100%" height={180}>
          <BarChart data={MONTHLY_SPEND}>
            <XAxis dataKey="month" tick={{ fill: "#94a3b8", fontSize: 11 }} />
            <YAxis tick={{ fill: "#64748b", fontSize: 11 }} />
            <Tooltip contentStyle={tooltipStyle} />
            <Bar dataKey="amount" fill="#eab308" radius={[6, 6, 0, 0]} />
          </BarChart>
        </ResponsiveContainer>
      </div>

      {/* Transaction log */}
      <div style={chartCardStyle}>
        <h3 style={chartTitleStyle}>Recent Transactions</h3>
        {[
          { date: "Feb 9", desc: "DJI Camera (Installment)", amt: "$498.00", tag: "Purchase" },
          { date: "Jan 27", desc: "Send money for Pakistan", amt: "$4,000.00", tag: "Transfer" },
          { date: "Jan 26", desc: "Groceries & supplies", amt: "$24.50", tag: "Food" },
          { date: "Jan 27", desc: "Transport", amt: "$17.00", tag: "Travel" },
        ].map((t, i) => (
          <div key={i} style={{ display: "flex", alignItems: "center", justifyContent: "space-between", padding: "12px 0", borderBottom: i < 3 ? "1px solid rgba(148,163,184,0.08)" : "none" }}>
            <div>
              <div style={{ color: "#e2e8f0", fontSize: 14, fontFamily: "'DM Sans', sans-serif" }}>{t.desc}</div>
              <div style={{ color: "#64748b", fontSize: 11, fontFamily: "'DM Sans', sans-serif" }}>{t.date} · {t.tag}</div>
            </div>
            <div style={{ color: "#ef4444", fontWeight: 600, fontSize: 14, fontFamily: "'Space Mono', monospace" }}>{t.amt}</div>
          </div>
        ))}
      </div>

      {/* Investment */}
      <div style={{ ...metricCardStyle, marginTop: 16, borderLeft: "3px solid #22c55e" }}>
        <div style={{ fontSize: 11, color: "#64748b", fontFamily: "'DM Sans', sans-serif", marginBottom: 4 }}>Robinhood Investing</div>
        <div style={{ fontSize: 24, fontWeight: 800, color: "#22c55e", fontFamily: "'DM Sans', sans-serif" }}>$800</div>
      </div>
    </div>
  );
}

function GoalsScreen() {
  const goals = [
    { type: "Daily", items: [
      { name: "Sleep ≥ 7 hours", progress: 100, done: true },
      { name: "Exercise ≥ 1 hour", progress: 100, done: true },
      { name: "Spend ≤ $20", progress: 100, done: true },
      { name: "Read 30 minutes", progress: 0, done: false },
    ]},
    { type: "Monthly", items: [
      { name: "Read 4 books", progress: 50, done: false },
      { name: "Work out 20+ days", progress: 65, done: false },
      { name: "Save $500", progress: 44, done: false },
    ]},
    { type: "Yearly", items: [
      { name: "Travel to 3 countries", progress: 0, done: false },
      { name: "Lose 5 kg", progress: 48, done: false },
      { name: "Complete 12 courses", progress: 17, done: false },
    ]},
  ];

  return (
    <div>
      <h2 style={h2Style}>Goals</h2>
      <p style={subtitleStyle}>Track daily, monthly, and yearly goals</p>

      {/* Streaks */}
      <div style={{ display: "flex", gap: 12, marginBottom: 24, overflowX: "auto" }}>
        {[
          { label: "Exercise Streak", value: "5 days", emoji: "🔥" },
          { label: "Logging Streak", value: "28 days", emoji: "📝" },
          { label: "Under Budget", value: "12 days", emoji: "💰" },
        ].map((s, i) => (
          <div key={i} style={{ ...metricCardStyle, minWidth: 140, textAlign: "center", flex: "1 0 auto" }}>
            <div style={{ fontSize: 28 }}>{s.emoji}</div>
            <div style={{ fontSize: 20, fontWeight: 800, color: "#f1f5f9", fontFamily: "'DM Sans', sans-serif" }}>{s.value}</div>
            <div style={{ fontSize: 11, color: "#64748b", fontFamily: "'DM Sans', sans-serif" }}>{s.label}</div>
          </div>
        ))}
      </div>

      {goals.map((group, gi) => (
        <div key={gi} style={{ marginBottom: 24 }}>
          <h3 style={{ color: "#e2e8f0", fontSize: 16, fontWeight: 600, fontFamily: "'DM Sans', sans-serif", margin: "0 0 12px" }}>{group.type} Goals</h3>
          <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
            {group.items.map((goal, i) => (
              <div key={i} style={{ background: "rgba(15,23,42,0.5)", borderRadius: 14, border: "1px solid rgba(148,163,184,0.08)", padding: 16 }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 8 }}>
                  <span style={{ color: "#e2e8f0", fontSize: 14, fontFamily: "'DM Sans', sans-serif", fontWeight: 500 }}>{goal.name}</span>
                  <span style={{ color: goal.done ? "#22c55e" : goal.progress > 50 ? "#eab308" : "#94a3b8", fontSize: 13, fontWeight: 700, fontFamily: "'Space Mono', monospace" }}>{goal.progress}%</span>
                </div>
                <div style={{ height: 8, borderRadius: 4, background: "rgba(148,163,184,0.1)", overflow: "hidden" }}>
                  <div style={{ height: "100%", width: `${goal.progress}%`, borderRadius: 4, background: goal.done ? "#22c55e" : goal.progress > 50 ? "linear-gradient(90deg, #eab308, #f59e0b)" : "linear-gradient(90deg, #6366f1, #818cf8)", transition: "width 0.8s ease" }} />
                </div>
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}

function ProductivityScreen() {
  return (
    <div>
      <h2 style={h2Style}>Productivity Command Center</h2>
      <p style={subtitleStyle}>Your morning briefing and daily planner</p>

      {/* Morning Briefing */}
      <div style={{ background: "linear-gradient(135deg, rgba(99,102,241,0.12), rgba(139,92,246,0.08))", borderRadius: 16, border: "1px solid rgba(99,102,241,0.2)", padding: 20, marginBottom: 20 }}>
        <div style={{ display: "flex", alignItems: "center", gap: 8, marginBottom: 14 }}>
          <span style={{ fontSize: 20 }}>☀️</span>
          <h3 style={{ color: "#a5b4fc", fontSize: 16, fontWeight: 600, margin: 0, fontFamily: "'DM Sans', sans-serif" }}>Morning Briefing — Feb 24</h3>
        </div>
        <div style={{ color: "#e2e8f0", fontSize: 14, lineHeight: 1.7, fontFamily: "'DM Sans', sans-serif" }}>
          <p style={{ margin: "0 0 8px" }}>🎯 <strong>3 goals</strong> active today: Sleep ≥7h ✅, Exercise ≥1h ⏳, Read 30min ⏳</p>
          <p style={{ margin: "0 0 8px" }}>🔥 Your <strong>exercise streak is at 5 days</strong> — don't break it!</p>
          <p style={{ margin: "0 0 8px" }}>📊 Yesterday you spent <strong>9h working</strong> — above average. Consider more rest today.</p>
          <p style={{ margin: 0, color: "#818cf8", fontStyle: "italic" }}>💡 "Consistency beats intensity. You've logged 28 days straight — that's the real achievement."</p>
        </div>
      </div>

      {/* Productivity Score */}
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginBottom: 20 }}>
        <div style={chartCardStyle}>
          <h3 style={chartTitleStyle}>Focus Score Today</h3>
          <div style={{ textAlign: "center", padding: "20px 0" }}>
            <div style={{ position: "relative", width: 120, height: 120, margin: "0 auto" }}>
              <svg width="120" height="120" viewBox="0 0 120 120">
                <circle cx="60" cy="60" r="52" fill="none" stroke="rgba(148,163,184,0.1)" strokeWidth="8" />
                <circle cx="60" cy="60" r="52" fill="none" stroke="#6366f1" strokeWidth="8" strokeDasharray={`${0.78 * 327} 327`} strokeLinecap="round" transform="rotate(-90 60 60)" />
              </svg>
              <div style={{ position: "absolute", inset: 0, display: "flex", alignItems: "center", justifyContent: "center", flexDirection: "column" }}>
                <span style={{ fontSize: 32, fontWeight: 800, color: "#f1f5f9", fontFamily: "'DM Sans', sans-serif" }}>78</span>
                <span style={{ fontSize: 10, color: "#64748b", fontFamily: "'DM Sans', sans-serif" }}>/ 100</span>
              </div>
            </div>
          </div>
        </div>
        <div style={chartCardStyle}>
          <h3 style={chartTitleStyle}>Planned vs Actual</h3>
          <ResponsiveContainer width="100%" height={140}>
            <BarChart data={[
              { slot: "Work", planned: 8, actual: 9 },
              { slot: "Exercise", planned: 2, actual: 1 },
              { slot: "Sleep", planned: 8, actual: 7 },
              { slot: "Social", planned: 2, actual: 3 },
            ]}>
              <XAxis dataKey="slot" tick={{ fill: "#94a3b8", fontSize: 10 }} />
              <YAxis tick={{ fill: "#64748b", fontSize: 10 }} />
              <Tooltip contentStyle={tooltipStyle} />
              <Bar dataKey="planned" fill="rgba(99,102,241,0.4)" radius={[4, 4, 0, 0]} />
              <Bar dataKey="actual" fill="#6366f1" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Task Queue */}
      <div style={chartCardStyle}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
          <h3 style={{ ...chartTitleStyle, margin: 0 }}>Smart Task Queue</h3>
          <span style={{ color: "#818cf8", fontSize: 11, fontFamily: "'DM Sans', sans-serif", cursor: "pointer" }}>+ Add Task</span>
        </div>
        {[
          { task: "Review PR #342", priority: "High", time: "9:00", done: true },
          { task: "Update project docs", priority: "Med", time: "11:00", done: false },
          { task: "Gym — leg day", priority: "High", time: "17:00", done: false },
          { task: "Read Clean Architecture ch.5", priority: "Low", time: "21:00", done: false },
        ].map((t, i) => (
          <div key={i} style={{ display: "flex", alignItems: "center", gap: 12, padding: "10px 0", borderBottom: i < 3 ? "1px solid rgba(148,163,184,0.06)" : "none" }}>
            <div style={{ width: 20, height: 20, borderRadius: 6, border: t.done ? "none" : "2px solid rgba(148,163,184,0.2)", background: t.done ? "#22c55e" : "transparent", display: "flex", alignItems: "center", justifyContent: "center", flexShrink: 0 }}>
              {t.done && <span style={{ color: "#fff", fontSize: 12 }}>✓</span>}
            </div>
            <div style={{ flex: 1 }}>
              <div style={{ color: t.done ? "#64748b" : "#e2e8f0", fontSize: 13, fontFamily: "'DM Sans', sans-serif", textDecoration: t.done ? "line-through" : "none" }}>{t.task}</div>
            </div>
            <span style={{ fontSize: 11, color: "#64748b", fontFamily: "'Space Mono', monospace" }}>{t.time}</span>
            <span style={{ fontSize: 10, padding: "2px 8px", borderRadius: 6, background: t.priority === "High" ? "rgba(239,68,68,0.15)" : t.priority === "Med" ? "rgba(234,179,8,0.15)" : "rgba(148,163,184,0.1)", color: t.priority === "High" ? "#ef4444" : t.priority === "Med" ? "#eab308" : "#94a3b8", fontWeight: 600, fontFamily: "'DM Sans', sans-serif" }}>{t.priority}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

function VideoLearningScreen() {
  const [view, setView] = useState("kanban");
  const kanban = {
    "To Start": [
      { title: "System Design Interview", platform: "YouTube", duration: "2h 30m", tags: ["Architecture"], priority: "High" },
      { title: "Advanced PostgreSQL", platform: "Udemy", duration: "8h", tags: ["Database"], priority: "Med" },
    ],
    "In Progress": [
      { title: "Clean Architecture in .NET", platform: "Pluralsight", duration: "4h", progress: 65, tags: ["C#", "Architecture"], priority: "High" },
      { title: "Blazor Deep Dive", platform: "YouTube", duration: "3h", progress: 30, tags: ["Blazor", ".NET"], priority: "High" },
    ],
    "Completed": [
      { title: "Docker Fundamentals", platform: "Udemy", duration: "6h", tags: ["DevOps"], priority: "Med" },
    ],
  };

  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 20 }}>
        <div>
          <h2 style={h2Style}>Video Learning Hub</h2>
          <p style={subtitleStyle}>Track courses, take notes, master skills</p>
        </div>
        <div style={{ display: "flex", gap: 6 }}>
          {["kanban", "list"].map(v => (
            <button key={v} onClick={() => setView(v)} style={{ padding: "6px 14px", borderRadius: 8, background: view === v ? "rgba(99,102,241,0.15)" : "transparent", border: view === v ? "1px solid rgba(99,102,241,0.3)" : "1px solid rgba(148,163,184,0.1)", color: view === v ? "#a5b4fc" : "#64748b", fontSize: 12, cursor: "pointer", fontFamily: "'DM Sans', sans-serif", textTransform: "capitalize" }}>{v}</button>
          ))}
        </div>
      </div>

      {/* Stats */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12, marginBottom: 20 }}>
        {[
          { label: "Total Videos", value: "5" },
          { label: "In Progress", value: "2" },
          { label: "Learning Hours", value: "23.5h" },
          { label: "Learning Streak", value: "3 days" },
        ].map((s, i) => (
          <div key={i} style={metricCardStyle}>
            <div style={{ fontSize: 20, fontWeight: 800, color: "#f1f5f9", fontFamily: "'DM Sans', sans-serif" }}>{s.value}</div>
            <div style={{ fontSize: 11, color: "#64748b", fontFamily: "'DM Sans', sans-serif" }}>{s.label}</div>
          </div>
        ))}
      </div>

      {/* Kanban */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 12 }}>
        {Object.entries(kanban).map(([status, items]) => (
          <div key={status} style={{ background: "rgba(15,23,42,0.3)", borderRadius: 14, border: "1px solid rgba(148,163,184,0.06)", padding: 12 }}>
            <div style={{ display: "flex", alignItems: "center", gap: 8, marginBottom: 12 }}>
              <div style={{ width: 8, height: 8, borderRadius: "50%", background: status === "To Start" ? "#64748b" : status === "In Progress" ? "#eab308" : "#22c55e" }} />
              <span style={{ color: "#94a3b8", fontSize: 12, fontWeight: 600, fontFamily: "'DM Sans', sans-serif" }}>{status}</span>
              <span style={{ color: "#475569", fontSize: 11, fontFamily: "'Space Mono', monospace" }}>{items.length}</span>
            </div>
            {items.map((item, i) => (
              <div key={i} style={{ background: "rgba(15,23,42,0.6)", borderRadius: 12, border: "1px solid rgba(148,163,184,0.08)", padding: 14, marginBottom: 8, cursor: "pointer", transition: "all 0.15s" }}>
                <div style={{ fontSize: 13, fontWeight: 600, color: "#e2e8f0", marginBottom: 6, fontFamily: "'DM Sans', sans-serif" }}>{item.title}</div>
                <div style={{ fontSize: 11, color: "#64748b", marginBottom: 8, fontFamily: "'DM Sans', sans-serif" }}>{item.platform} · {item.duration}</div>
                {item.progress !== undefined && (
                  <div style={{ marginBottom: 8 }}>
                    <div style={{ height: 4, borderRadius: 2, background: "rgba(148,163,184,0.1)", overflow: "hidden" }}>
                      <div style={{ height: "100%", width: `${item.progress}%`, borderRadius: 2, background: "#eab308" }} />
                    </div>
                    <div style={{ fontSize: 10, color: "#64748b", marginTop: 3, fontFamily: "'Space Mono', monospace" }}>{item.progress}%</div>
                  </div>
                )}
                <div style={{ display: "flex", flexWrap: "wrap", gap: 4 }}>
                  {item.tags.map(t => (
                    <span key={t} style={{ fontSize: 10, padding: "2px 8px", borderRadius: 6, background: "rgba(99,102,241,0.1)", color: "#818cf8", fontFamily: "'DM Sans', sans-serif" }}>{t}</span>
                  ))}
                  <span style={{ fontSize: 10, padding: "2px 8px", borderRadius: 6, background: item.priority === "High" ? "rgba(239,68,68,0.12)" : "rgba(234,179,8,0.12)", color: item.priority === "High" ? "#ef4444" : "#eab308", fontFamily: "'DM Sans', sans-serif" }}>{item.priority}</span>
                </div>
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  );
}

function BooksScreen() {
  const books = [
    { title: "Clean Architecture", author: "Robert C. Martin", pages: 432, current: 432, status: "Completed", rating: 5, genre: "Engineering" },
    { title: "Designing Data-Intensive Applications", author: "Martin Kleppmann", pages: 616, current: 310, status: "In Progress", rating: null, genre: "Systems" },
    { title: "Atomic Habits", author: "James Clear", pages: 320, current: 0, status: "To Start", rating: null, genre: "Self-Help" },
    { title: "The Pragmatic Programmer", author: "Hunt & Thomas", pages: 352, current: 200, status: "In Progress", rating: null, genre: "Engineering" },
  ];

  return (
    <div>
      <h2 style={h2Style}>Book Reading List</h2>
      <p style={subtitleStyle}>Track reading progress and capture knowledge</p>

      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12, marginBottom: 20 }}>
        {[
          { label: "Total Books", value: "4" },
          { label: "Completed", value: "1" },
          { label: "Pages Read", value: "942" },
          { label: "Reading Pace", value: "15 pg/day" },
        ].map((s, i) => (
          <div key={i} style={metricCardStyle}>
            <div style={{ fontSize: 20, fontWeight: 800, color: "#f1f5f9", fontFamily: "'DM Sans', sans-serif" }}>{s.value}</div>
            <div style={{ fontSize: 11, color: "#64748b", fontFamily: "'DM Sans', sans-serif" }}>{s.label}</div>
          </div>
        ))}
      </div>

      {books.map((b, i) => (
        <div key={i} style={{ background: "rgba(15,23,42,0.5)", borderRadius: 14, border: "1px solid rgba(148,163,184,0.08)", padding: 16, marginBottom: 10, display: "flex", gap: 16, alignItems: "center" }}>
          <div style={{ width: 48, height: 64, borderRadius: 6, background: `linear-gradient(135deg, ${["#6366f1","#2563eb","#16a34a","#d4a373"][i]}, ${["#8b5cf6","#3b82f6","#22c55e","#e8a87c"][i]})`, display: "flex", alignItems: "center", justifyContent: "center", flexShrink: 0 }}>
            <span style={{ color: "#fff", fontSize: 18, fontWeight: 800, fontFamily: "'Space Mono', monospace" }}>{b.title[0]}</span>
          </div>
          <div style={{ flex: 1 }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "start" }}>
              <div>
                <div style={{ color: "#e2e8f0", fontSize: 14, fontWeight: 600, fontFamily: "'DM Sans', sans-serif" }}>{b.title}</div>
                <div style={{ color: "#64748b", fontSize: 12, fontFamily: "'DM Sans', sans-serif" }}>{b.author} · {b.genre}</div>
              </div>
              <span style={{ fontSize: 10, padding: "3px 10px", borderRadius: 8, background: b.status === "Completed" ? "rgba(34,197,94,0.12)" : b.status === "In Progress" ? "rgba(234,179,8,0.12)" : "rgba(148,163,184,0.08)", color: b.status === "Completed" ? "#22c55e" : b.status === "In Progress" ? "#eab308" : "#94a3b8", fontWeight: 600, fontFamily: "'DM Sans', sans-serif" }}>{b.status}</span>
            </div>
            <div style={{ marginTop: 8 }}>
              <div style={{ height: 6, borderRadius: 3, background: "rgba(148,163,184,0.08)", overflow: "hidden" }}>
                <div style={{ height: "100%", width: `${(b.current / b.pages) * 100}%`, borderRadius: 3, background: b.status === "Completed" ? "#22c55e" : "#6366f1" }} />
              </div>
              <div style={{ display: "flex", justifyContent: "space-between", marginTop: 4 }}>
                <span style={{ fontSize: 10, color: "#64748b", fontFamily: "'Space Mono', monospace" }}>{b.current} / {b.pages} pages</span>
                {b.rating && <span style={{ fontSize: 10, color: "#eab308" }}>{"★".repeat(b.rating)}</span>}
              </div>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}

function SettingsScreen() {
  return (
    <div>
      <h2 style={h2Style}>Settings</h2>
      <p style={subtitleStyle}>Configure your tracking preferences</p>

      {[
        { section: "Profile", items: [
          { label: "Name", value: "User", type: "text" },
          { label: "Email", value: "user@example.com", type: "text" },
          { label: "Timezone", value: "America/New_York", type: "select" },
        ]},
        { section: "Tracking", items: [
          { label: "Time Granularity", value: "1 Hour", type: "select", options: ["15 Min", "30 Min", "1 Hour"] },
          { label: "Tracking Period", value: "Jan 1 – Dec 31, 2026", type: "text" },
          { label: "Monthly Budget", value: "$500", type: "text" },
        ]},
        { section: "AI Features", items: [
          { label: "Auto-Fill Day Logger", value: true, type: "toggle" },
          { label: "Smart Suggestions", value: true, type: "toggle" },
          { label: "Daily Summary Generator", value: false, type: "toggle" },
          { label: "Spending Insights", value: true, type: "toggle" },
          { label: "Goal Coaching", value: true, type: "toggle" },
          { label: "Auto-Fill Confidence Threshold", value: "70%", type: "select" },
        ]},
        { section: "Notifications", items: [
          { label: "Reminder Interval", value: "Every 3 hours", type: "select" },
          { label: "Quiet Hours", value: "11 PM – 7 AM", type: "text" },
          { label: "Push Notifications", value: true, type: "toggle" },
        ]},
        { section: "Appearance", items: [
          { label: "Theme", value: "Dark", type: "select", options: ["Dark", "Light"] },
        ]},
        { section: "Data", items: [
          { label: "Export Data", value: "Excel / CSV / PDF", type: "button" },
          { label: "Import from Excel", value: "Upload .xlsx", type: "button" },
        ]},
      ].map((group, gi) => (
        <div key={gi} style={{ marginBottom: 20 }}>
          <h3 style={{ color: "#a5b4fc", fontSize: 12, fontWeight: 700, fontFamily: "'DM Sans', sans-serif", textTransform: "uppercase", letterSpacing: "0.06em", margin: "0 0 10px" }}>{group.section}</h3>
          <div style={{ background: "rgba(15,23,42,0.5)", borderRadius: 14, border: "1px solid rgba(148,163,184,0.08)", overflow: "hidden" }}>
            {group.items.map((item, i) => (
              <div key={i} style={{ display: "flex", alignItems: "center", justifyContent: "space-between", padding: "14px 16px", borderBottom: i < group.items.length - 1 ? "1px solid rgba(148,163,184,0.06)" : "none" }}>
                <span style={{ color: "#e2e8f0", fontSize: 13, fontFamily: "'DM Sans', sans-serif" }}>{item.label}</span>
                {item.type === "toggle" ? (
                  <div style={{ width: 44, height: 24, borderRadius: 12, background: item.value ? "#6366f1" : "rgba(148,163,184,0.15)", cursor: "pointer", position: "relative", transition: "all 0.2s" }}>
                    <div style={{ width: 18, height: 18, borderRadius: "50%", background: "#fff", position: "absolute", top: 3, left: item.value ? 23 : 3, transition: "left 0.2s" }} />
                  </div>
                ) : item.type === "button" ? (
                  <button style={{ padding: "6px 14px", borderRadius: 8, background: "rgba(99,102,241,0.1)", border: "1px solid rgba(99,102,241,0.2)", color: "#818cf8", fontSize: 12, cursor: "pointer", fontFamily: "'DM Sans', sans-serif" }}>{item.value}</button>
                ) : (
                  <span style={{ color: "#94a3b8", fontSize: 13, fontFamily: "'DM Sans', sans-serif" }}>{item.value}</span>
                )}
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}

// ─── Shared Styles ───
const h2Style = { color: "#f1f5f9", fontSize: 24, fontWeight: 700, margin: "0 0 4px", fontFamily: "'Playfair Display', Georgia, serif", letterSpacing: "-0.02em" };
const subtitleStyle = { color: "#64748b", fontSize: 13, margin: "0 0 20px", fontFamily: "'DM Sans', sans-serif" };
const btnStyle = { padding: "10px 18px", borderRadius: 10, border: "none", fontSize: 13, fontWeight: 600, cursor: "pointer", fontFamily: "'DM Sans', sans-serif", display: "flex", alignItems: "center", gap: 6 };
const metricCardStyle = { background: "rgba(15,23,42,0.5)", borderRadius: 14, border: "1px solid rgba(148,163,184,0.08)", padding: 16, display: "flex", flexDirection: "column", gap: 4 };
const chartCardStyle = { background: "rgba(15,23,42,0.5)", borderRadius: 16, border: "1px solid rgba(148,163,184,0.08)", padding: 20 };
const chartTitleStyle = { color: "#94a3b8", fontSize: 13, fontWeight: 600, margin: "0 0 12px", fontFamily: "'DM Sans', sans-serif" };
const tooltipStyle = { background: "#1e293b", border: "1px solid rgba(148,163,184,0.15)", borderRadius: 10, color: "#e2e8f0", fontSize: 12, fontFamily: "'DM Sans', sans-serif" };

// ─── Main App ───
export default function LifeTrackerPro() {
  const [loggedIn, setLoggedIn] = useState(false);
  const [activeScreen, setActiveScreen] = useState("daily");

  if (!loggedIn) return <LoginScreen onLogin={() => setLoggedIn(true)} />;

  const navItems = [
    { id: "daily", label: "Logger", icon: Icons.grid },
    { id: "dashboard", label: "Dashboard", icon: Icons.chart },
    { id: "spending", label: "Spending", icon: Icons.dollar },
    { id: "goals", label: "Goals", icon: Icons.target },
    { id: "productivity", label: "Productivity", icon: Icons.zap },
    { id: "videos", label: "Videos", icon: Icons.play },
    { id: "books", label: "Books", icon: Icons.book },
    { id: "settings", label: "Settings", icon: Icons.settings },
  ];

  const screens = {
    daily: <DailyLogScreen />,
    dashboard: <DashboardScreen />,
    spending: <SpendingScreen />,
    goals: <GoalsScreen />,
    productivity: <ProductivityScreen />,
    videos: <VideoLearningScreen />,
    books: <BooksScreen />,
    settings: <SettingsScreen />,
  };

  return (
    <div style={{ minHeight: "100vh", background: "#0c0a1a", display: "flex" }}>
      <style>{`
        @import url('https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;600;700;800&family=DM+Sans:wght@400;500;600;700&family=Space+Mono:wght@400;700&display=swap');
        * { box-sizing: border-box; margin: 0; padding: 0; }
        ::-webkit-scrollbar { width: 6px; height: 6px; }
        ::-webkit-scrollbar-track { background: transparent; }
        ::-webkit-scrollbar-thumb { background: rgba(148,163,184,0.15); border-radius: 3px; }
        @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
        .spin { display: inline-block; animation: spin 1s linear infinite; }
      `}</style>

      {/* Sidebar */}
      <nav style={{ width: 220, background: "rgba(15,23,42,0.6)", borderRight: "1px solid rgba(148,163,184,0.06)", padding: "20px 12px", display: "flex", flexDirection: "column", flexShrink: 0 }}>
        <div style={{ display: "flex", alignItems: "center", gap: 10, padding: "0 8px", marginBottom: 28 }}>
          <div style={{ width: 36, height: 36, borderRadius: 10, background: "linear-gradient(135deg, #6366f1, #8b5cf6)", display: "flex", alignItems: "center", justifyContent: "center" }}>
            <span style={{ color: "#fff", fontSize: 18, fontWeight: 800 }}>L</span>
          </div>
          <div>
            <span style={{ color: "#f1f5f9", fontSize: 16, fontWeight: 700, fontFamily: "'Playfair Display', Georgia, serif" }}>LifeTracker</span>
            <span style={{ display: "block", color: "#818cf8", fontSize: 9, fontWeight: 600, fontFamily: "'Space Mono', monospace", letterSpacing: "0.1em" }}>PRO</span>
          </div>
        </div>

        {navItems.map(item => (
          <button key={item.id} onClick={() => setActiveScreen(item.id)} style={{ display: "flex", alignItems: "center", gap: 10, padding: "10px 12px", borderRadius: 10, border: "none", background: activeScreen === item.id ? "rgba(99,102,241,0.12)" : "transparent", color: activeScreen === item.id ? "#a5b4fc" : "#64748b", fontSize: 13, fontWeight: activeScreen === item.id ? 600 : 400, cursor: "pointer", fontFamily: "'DM Sans', sans-serif", marginBottom: 2, width: "100%", textAlign: "left", transition: "all 0.15s" }}>
            {item.icon}
            {item.label}
          </button>
        ))}

        <div style={{ flex: 1 }} />
        <div style={{ padding: "12px", background: "rgba(99,102,241,0.06)", borderRadius: 12, border: "1px solid rgba(99,102,241,0.1)" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 8, marginBottom: 4 }}>
            <div style={{ width: 28, height: 28, borderRadius: 8, background: "rgba(99,102,241,0.2)", display: "flex", alignItems: "center", justifyContent: "center", color: "#818cf8" }}>{Icons.user}</div>
            <div>
              <div style={{ color: "#e2e8f0", fontSize: 12, fontWeight: 600, fontFamily: "'DM Sans', sans-serif" }}>User</div>
              <div style={{ color: "#64748b", fontSize: 10, fontFamily: "'DM Sans', sans-serif" }}>28-day streak 🔥</div>
            </div>
          </div>
        </div>
      </nav>

      {/* Main content */}
      <main style={{ flex: 1, padding: 28, overflowY: "auto", maxHeight: "100vh" }}>
        {screens[activeScreen]}
      </main>
    </div>
  );
}
