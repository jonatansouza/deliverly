import React from "react";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import Calculator from "./components/Calculator";
import Header from "./components/Header";
import History from "./components/History";

const App: React.FC = () => {
  return (
    <Router>
      <div className="min-h-screen bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-slate-50 via-white to-indigo-50/30">
        <Header />

        <main>
          <Routes>
            <Route path="/" element={<History />} />
            <Route path="/calculate" element={<Calculator />} />
            <Route
              path="*"
              element={<div className="p-8">Page not found</div>}
            />
          </Routes>
        </main>
      </div>
    </Router>
  );
};

export default App;
