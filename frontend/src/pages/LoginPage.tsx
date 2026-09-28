import { useState } from "react";
import { login, type Session } from "../auth/authService";

export default function LoginPage({ onLogin }: { onLogin: (session: Session) => void }) {
  const [usuario, setUsuario] = useState("admin");
  const [password, setPassword] = useState("admin");
  const [error, setError] = useState("");
  const [cargando, setCargando] = useState(false);

  async function submit(e: React.FormEvent) {
    e.preventDefault(); setError(""); setCargando(true);
    try { onLogin(await login(usuario, password)); }
    catch (e) { setError(e instanceof Error ? e.message : "No se pudo iniciar sesión."); }
    finally { setCargando(false); }
  }

  return <div className="login-page"><form className="login-card" onSubmit={submit}>
    <h1>Tercceros</h1><p>Gestión Académica</p>
    <div className="form-group"><label>Usuario</label><input value={usuario} onChange={e=>setUsuario(e.target.value)} autoFocus /></div>
    <div className="form-group"><label>Contraseña</label><input type="password" value={password} onChange={e=>setPassword(e.target.value)} /></div>
    {error && <div className="alert-error">{error}</div>}
    <button className="btn btn-primary login-button" disabled={cargando}>{cargando ? "Ingresando..." : "Ingresar"}</button>
    <small>Prueba local inicial: admin / admin</small>
  </form></div>;
}
