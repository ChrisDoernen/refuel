import { Outlet } from "react-router";

export default function AuthLayout() {
  return (
    <div>
      <h1>Auth</h1>

      <Outlet />
    </div>
  );
}
