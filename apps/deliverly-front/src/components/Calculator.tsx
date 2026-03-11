import {
  ArrowDownUp,
  Calculator as CalcIcon,
  MapPin,
  Navigation,
  Package,
} from "lucide-react";
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { ticketService } from "../services/ticket.service";
import type { TicketRequest } from "../types/ticket";

const INITIAL_FORM: TicketRequest = { origin: "", destination: "", ticket: "" };

const Calculator: React.FC = () => {
  const { t } = useTranslation();
  const [form, setForm] = useState<TicketRequest>(INITIAL_FORM);
  const [loading, setLoading] = useState(false);
  const [feedback, setFeedback] = useState<{ ok: boolean; msg: string } | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    setFeedback(null);
  };

  const handleSwap = () => {
    setForm((prev) => ({ ...prev, origin: prev.destination, destination: prev.origin }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setFeedback(null);
    try {
      await ticketService.create(form);
      setFeedback({ ok: true, msg: t("calculate.success") });
      setForm(INITIAL_FORM);
    } catch {
      setFeedback({ ok: false, msg: t("calculate.error") });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative isolate px-6 pt-14 lg:px-8">
      <div className="mx-auto max-w-2xl py-12">
        <div className="relative overflow-hidden bg-white/40 backdrop-blur-xl border border-white/40 rounded-3xl shadow-2xl p-8 md:p-12">
          <div className="absolute -top-24 -right-24 w-48 h-48 bg-indigo-500/20 rounded-full blur-3xl" />
          <div className="absolute -bottom-24 -left-24 w-48 h-48 bg-violet-500/20 rounded-full blur-3xl" />

          <div className="relative">
            <header className="mb-10 text-center">
              <div className="inline-flex items-center justify-center p-3 bg-indigo-600 rounded-2xl shadow-lg mb-4">
                <Package className="text-white" size={28} />
              </div>
              <h1 className="text-3xl font-extrabold text-slate-900 tracking-tight">
                {t("calculate.title")}
              </h1>
              <p className="text-slate-500 mt-2 font-medium">
                {t("calculate.subtitle")}
              </p>
            </header>

            <form className="space-y-6" onSubmit={handleSubmit}>
              {/* Origin */}
              <div className="space-y-2">
                <label className="text-sm font-bold text-slate-700 ml-1 flex items-center gap-2">
                  <Navigation size={14} className="text-indigo-500" />
                  {t("calculate.from")}
                </label>
                <div className="relative group">
                  <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                    <MapPin className="text-slate-400 group-focus-within:text-indigo-500 transition-colors" size={20} />
                  </div>
                  <input
                    name="origin"
                    type="text"
                    value={form.origin}
                    onChange={handleChange}
                    placeholder={t("calculate.placeholder")}
                    required
                    className="block w-full pl-11 pr-4 py-4 bg-white/60 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 transition-all outline-none text-slate-800 placeholder:text-slate-400 font-medium"
                  />
                </div>
              </div>

              {/* Swap */}
              <div className="flex justify-center -my-2 relative z-10">
                <button
                  type="button"
                  onClick={handleSwap}
                  className="p-2 bg-white border border-slate-200 rounded-full shadow-md text-slate-400 hover:text-indigo-600 hover:border-indigo-200 transition-all active:scale-90"
                >
                  <ArrowDownUp size={20} />
                </button>
              </div>

              {/* Destination */}
              <div className="space-y-2">
                <label className="text-sm font-bold text-slate-700 ml-1 flex items-center gap-2">
                  <MapPin size={14} className="text-indigo-500" />
                  {t("calculate.to")}
                </label>
                <div className="relative group">
                  <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                    <MapPin className="text-slate-400 group-focus-within:text-indigo-500 transition-colors" size={20} />
                  </div>
                  <input
                    name="destination"
                    type="text"
                    value={form.destination}
                    onChange={handleChange}
                    placeholder={t("calculate.placeholder")}
                    required
                    className="block w-full pl-11 pr-4 py-4 bg-white/60 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 transition-all outline-none text-slate-800 placeholder:text-slate-400 font-medium"
                  />
                </div>
              </div>

              {/* Ticket type */}
              <div className="space-y-2">
                <label className="text-sm font-bold text-slate-700 ml-1 flex items-center gap-2">
                  <Package size={14} className="text-indigo-500" />
                  {t("calculate.ticket")}
                </label>
                <div className="relative group">
                  <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                    <Package className="text-slate-400 group-focus-within:text-indigo-500 transition-colors" size={20} />
                  </div>
                  <input
                    name="ticket"
                    type="text"
                    value={form.ticket}
                    onChange={handleChange}
                    placeholder={t("calculate.ticketPlaceholder")}
                    required
                    className="block w-full pl-11 pr-4 py-4 bg-white/60 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 transition-all outline-none text-slate-800 placeholder:text-slate-400 font-medium"
                  />
                </div>
              </div>

              {/* Feedback */}
              {feedback && (
                <p className={`text-sm font-medium text-center ${feedback.ok ? "text-green-600" : "text-red-500"}`}>
                  {feedback.msg}
                </p>
              )}

              {/* Submit */}
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-slate-900 hover:bg-indigo-600 disabled:opacity-60 disabled:cursor-not-allowed text-white font-bold py-4 rounded-2xl shadow-xl shadow-slate-200 transition-all duration-300 hover:-translate-y-1 active:scale-95 flex items-center justify-center gap-2"
              >
                <CalcIcon size={20} />
                {loading ? t("calculate.loading") : t("calculate.button")}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Calculator;
