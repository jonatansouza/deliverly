import { Bell, Calculator, History, MapPin, Settings } from "lucide-react"; // Adicionei Settings
import React from "react";
import { useTranslation } from "react-i18next";
import { Link, NavLink } from "react-router-dom";

interface NavLinkItem {
  label: string;
  href: string;
  icon: React.ReactNode;
}

const Header: React.FC = () => {
  const { t, i18n } = useTranslation();

  // Função para alternar entre PT e EN
  const toggleLanguage = () => {
    const newLanguage = i18n.language === "pt" ? "en" : "pt";
    i18n.changeLanguage(newLanguage);
  };

  const links: NavLinkItem[] = [
    { label: t("header.history"), href: "/", icon: <History size={18} /> },
    {
      label: t("header.calculate"),
      href: "/calculate",
      icon: <Calculator size={18} />,
    },
  ];

  return (
    <nav className="sticky top-0 z-50 bg-white/80 backdrop-blur-md border-b border-slate-200/60 font-sans">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          {/* Logo */}
          <Link to="/" className="flex items-center gap-3 group cursor-pointer">
            <div className="w-10 h-10 bg-slate-900 rounded-xl flex items-center justify-center shadow-lg shadow-slate-200 group-hover:bg-indigo-600 transition-all duration-300">
              <MapPin className="text-white" size={22} />
            </div>
            <span className="text-2xl font-extrabold tracking-tighter text-slate-900">
              Deliverly<span className="text-indigo-600">.</span>
            </span>
          </Link>

          {/* Navigation */}
          <div className="hidden md:flex items-center gap-1">
            {links.map((link) => (
              <NavLink
                key={link.label}
                to={link.href}
                className={({ isActive }) =>
                  `flex items-center gap-2 px-4 py-2 text-sm font-semibold transition-all rounded-lg ${
                    isActive
                      ? "text-indigo-600 bg-indigo-50/80 shadow-sm shadow-indigo-100"
                      : "text-slate-600 hover:text-indigo-600 hover:bg-indigo-50/50"
                  }`
                }
              >
                {link.icon}
                {link.label}
              </NavLink>
            ))}
          </div>

          {/* Actions & Settings */}
          <div className="flex items-center gap-2">
            {/* Botão de Troca de Idioma / Configurações */}
            <button
              onClick={toggleLanguage}
              title="Change Language"
              className="group flex items-center gap-2 p-2.5 text-slate-500 hover:text-indigo-600 hover:bg-indigo-50 rounded-full transition-all border border-transparent hover:border-indigo-100"
            >
              <Settings
                size={20}
                className="group-hover:rotate-90 transition-transform duration-500"
              />
              <span className="text-xs font-black uppercase tracking-widest bg-slate-100 px-1.5 py-0.5 rounded text-slate-500 group-hover:bg-indigo-100 group-hover:text-indigo-600 transition-colors">
                {i18n.language.substring(0, 2)}
              </span>
            </button>

            <button className="relative p-2.5 text-slate-500 hover:text-indigo-600 hover:bg-indigo-50 rounded-full transition-all">
              <Bell size={22} />
              <span className="absolute top-2 right-2.5 w-2 h-2 bg-red-500 border-2 border-white rounded-full"></span>
            </button>

            <div className="h-6 w-[1px] bg-slate-200 mx-2"></div>

            <button className="hidden sm:block text-sm font-bold text-slate-700 hover:text-indigo-600 px-4 py-2">
              {t("header.myAccount")}
            </button>
            <button className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-bold px-6 py-2.5 rounded-xl transition-all shadow-lg shadow-indigo-100 active:scale-95">
              {t("header.track")}
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Header;
