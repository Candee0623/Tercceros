import { useMemo, useState } from "react";
import {
  BrowserRouter,
  Navigate,
  NavLink,
  Route,
  Routes,
} from "react-router-dom";

import ProfesoresPage from "./pages/ProfesoresPage";
import MateriasPage from "./pages/MateriasPage";
import AlumnosPage from "./pages/AlumnosPage";
import CarrerasPage from "./pages/CarrerasPage";
import UsuariosPage from "./pages/UsuariosPage";
import RolesPage from "./pages/RolesPage";
import LoginPage from "./pages/LoginPage";
import CursadasPage from "./pages/CursadasPage";
import MatriculacionesPage from "./pages/MatriculacionesPage";
import AsistenciasPage from "./pages/AsistenciasPage";
import EstadoAsistenciaAlumnosPage from "./pages/EstadoAsistenciaAlumnosPage";
import MisDatosPage from "./pages/MisDatosPage";
import MiAsistenciaPage from "./pages/MiAsistenciaPage";
import AutoMatriculacionPage from "./pages/AutoMatriculacionPage";
import CalificacionesPage from "./pages/CalificacionesPage";
import CuotasPage from "./pages/CuotasPage";
import PlanEstudiosPage from "./pages/PlanEstudiosPage";
import CalendarioPage from "./pages/CalendarioPage";
import MensajesPage from "./pages/MensajesPage";

import {
  clearSession,
  getSession,
  type Session,
} from "./auth/authService";

import "./index.css";

type MenuItem = {
  path: string;
  label: string;
  icon: string;
  permiso: string;
  element: React.ReactNode;
  studentOnly?: boolean;
};

function App() {
  const [menuColapsado, setMenuColapsado] = useState(false);

  const [session, setSession] = useState<Session | null>(
    () => getSession()
  );

  const items: MenuItem[] = useMemo(
    () => [
      // Autogestión del alumno
      {
        path: "/mis-datos",
        label: "Mis datos",
        icon: "🙋",
        permiso: "mis_datos",
        element: <MisDatosPage />,
        studentOnly: true,
      },
      {
        path: "/mi-asistencia",
        label: "Mi asistencia",
        icon: "📊",
        permiso: "mi_asistencia",
        element: <MiAsistenciaPage />,
        studentOnly: true,
      },
      {
        path: "/inscripcion-materias",
        label: "Inscripción a materias",
        icon: "📝",
        permiso: "auto_matriculacion",
        element: <AutoMatriculacionPage />,
        studentOnly: true,
      },

      // Gestión administrativa/académica
      {
        path: "/alumnos",
        label: "Alumnos",
        icon: "👨‍🎓",
        permiso: "alumnos",
        element: <AlumnosPage />,
      },
      {
        path: "/carreras",
        label: "Carreras",
        icon: "🎓",
        permiso: "carreras",
        element: <CarrerasPage />,
      },
      {
        path: "/profesores",
        label: "Profesores",
        icon: "👨‍🏫",
        permiso: "profesores",
        element: <ProfesoresPage />,
      },
      {
        path: "/materias",
        label: "Materias",
        icon: "📚",
        permiso: "materias",
        element: <MateriasPage />,
      },
      {
        path: "/plan-estudios",
        label: "Plan de estudios",
        icon: "🗂️",
        permiso: "plan_estudios",
        element: <PlanEstudiosPage />,
      },
      {
        path: "/cursadas",
        label: "Cursadas",
        icon: "🗓️",
        permiso: "cursadas",
        element: <CursadasPage />,
      },
      {
        path: "/mensajes",
        label: "Mensajes",
        icon: "💬",
        permiso: "mensajes",
        element: <MensajesPage />,
      },
      {
        path: "/calendario",
        label: "Calendario",
        icon: "📅",
        permiso: "calendario",
        element: <CalendarioPage />,
      },
      {
        path: "/matriculaciones",
        label: "Matriculaciones",
        icon: "📝",
        permiso: "matriculaciones",
        element: <MatriculacionesPage />,
      },
      {
        path: "/asistencias",
        label: "Asistencia",
        icon: "✅",
        permiso: "asistencias",
        element: <AsistenciasPage />,
      },
      {
        path: "/estado-asistencia",
        label: "Estado asistencia",
        icon: "📈",
        permiso: "estado_asistencia",
        element: <EstadoAsistenciaAlumnosPage />,
      },
      {
        path: "/calificaciones",
        label: "Calificaciones",
        icon: "🧾",
        permiso: "calificaciones",
        element: <CalificacionesPage />,
      },
      {
        path: "/cuotas",
        label: "Cuotas",
        icon: "💳",
        permiso: "cuotas",
        element: <CuotasPage />,
      },
      {
        path: "/usuarios",
        label: "Usuarios",
        icon: "👤",
        permiso: "usuarios",
        element: <UsuariosPage />,
      },
      {
        path: "/roles",
        label: "Roles",
        icon: "🔐",
        permiso: "roles",
        element: <RolesPage />,
      },
    ],
    []
  );

  if (!session) {
    return <LoginPage onLogin={setSession} />;
  }

  const visibles = items.filter(
    (item) =>
      session.permisos.includes(item.permiso) &&
      (!item.studentOnly || !!session.alumnoId)
  );

  const inicio = visibles[0]?.path ?? "/sin-acceso";

  function logout() {
    clearSession();
    setSession(null);
  }

  return (
    <BrowserRouter>
      <div className="app-shell">
        <aside
          className={`sidebar ${
            menuColapsado ? "collapsed" : ""
          }`}
        >
          <div className="sidebar-top">
            {!menuColapsado && (
              <div className="brand">
                <h2>Tercceros</h2>
                <span>Gestión Académica</span>
              </div>
            )}

            <button
              className="collapse-button"
              onClick={() =>
                setMenuColapsado(!menuColapsado)
              }
              title={
                menuColapsado
                  ? "Abrir menú"
                  : "Cerrar menú"
              }
            >
              {menuColapsado ? "→" : "←"}
            </button>
          </div>

          <nav className="menu">
            {visibles.map((item) => (
              <NavLink
                key={item.path}
                to={item.path}
                className={({ isActive }) =>
                  `menu-item ${
                    isActive ? "active" : ""
                  }`
                }
              >
                <span className="menu-icon">
                  {item.icon}
                </span>

                {!menuColapsado && (
                  <span>{item.label}</span>
                )}
              </NavLink>
            ))}
          </nav>

          <div className="sidebar-user">
            {!menuColapsado && (
              <div>
                <strong>
                  {session.nombreUsuario}
                </strong>

                <small>
                  {session.rol}
                  {session.alumnoId
                    ? " · Alumno"
                    : ""}
                </small>
              </div>
            )}

            <button
              className="logout-button"
              onClick={logout}
              title="Cerrar sesión"
            >
              ⏻
            </button>
          </div>
        </aside>

        <main className="main-content">
          <Routes>
            <Route
              path="/"
              element={
                <Navigate
                  to={inicio}
                  replace
                />
              }
            />

            {items.map((item) => (
              <Route
                key={item.path}
                path={item.path}
                element={
                  session.permisos.includes(
                    item.permiso
                  ) &&
                  (!item.studentOnly ||
                    !!session.alumnoId) ? (
                    item.element
                  ) : (
                    <Navigate
                      to={inicio}
                      replace
                    />
                  )
                }
              />
            ))}

            <Route
              path="/sin-acceso"
              element={
                <div className="card">
                  <h2>Sin accesos asignados</h2>

                  <p>
                    Tu rol no tiene pantallas
                    habilitadas. Contactá a un
                    administrador.
                  </p>
                </div>
              }
            />

            <Route
              path="*"
              element={
                <Navigate
                  to={inicio}
                  replace
                />
              }
            />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;