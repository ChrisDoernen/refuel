import { StrictMode, Suspense } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { RelayEnvironmentProvider } from "react-relay";
import { Environment, Network, type FetchFunction } from "relay-runtime";
import { BrowserRouter, Route, Routes } from 'react-router';
import Home from "./Home.tsx";
import Signup from "./Signup.tsx";
import AuthLayout from "./AuthLayout.tsx";
import Clubs from "./Clubs.tsx";

const HTTP_ENDPOINT = "/api/graphql/";

const fetchGraphQL: FetchFunction = async (request, variables) => {
  const resp = await fetch(HTTP_ENDPOINT, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ query: request.text, variables }),
  });
  if (!resp.ok) {
    throw new Error("Response failed.");
  }
  return await resp.json();
};

const environment = new Environment({
  network: Network.create(fetchGraphQL),
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <RelayEnvironmentProvider environment={environment}>
      <Suspense fallback="Loading...">
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route element={<AuthLayout />}>
              <Route path="/signup" element={<Signup />} />
            </Route>
              <Route path="/clubs" element={<Clubs />} />
          </Routes>
        </BrowserRouter>
      </Suspense>
    </RelayEnvironmentProvider>
  </StrictMode>
);