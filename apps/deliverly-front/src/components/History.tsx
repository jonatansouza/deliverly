import { ArrowRight, ChevronRight, Clock, Inbox } from "lucide-react";
import React from "react";
import { useTranslation } from "react-i18next";

// Interface para os dados do histórico
interface FreightCalculation {
  id: string;
  from: string;
  to: string;
  date: string;
  price: string;
  status: "completed" | "pending" | "expired";
}

const History: React.FC = () => {
  const { t } = useTranslation();

  // Mock de dados para visualização inicial
  const calculations: FreightCalculation[] = [
    {
      id: "1",
      from: "São Paulo, SP",
      to: "Rio de Janeiro, RJ",
      date: "2026-03-02",
      price: "R$ 450,00",
      status: "completed",
    },
    {
      id: "2",
      from: "Araruama, RJ",
      to: "Cabo Frio, RJ",
      date: "2026-03-01",
      price: "R$ 120,00",
      status: "completed",
    },
  ];

  return (
    <div className="max-w-5xl mx-auto px-4 py-12">
      <header className="mb-10">
        <h1 className="text-3xl font-extrabold text-slate-900 tracking-tight">
          {t("history.title")}
        </h1>
        <p className="text-slate-500 mt-2 font-medium italic">
          {t("history.subtitle")}
        </p>
      </header>

      {calculations.length > 0 ? (
        <div className="grid gap-4">
          {calculations.map((item) => (
            <div
              key={item.id}
              className="group relative overflow-hidden bg-white/60 backdrop-blur-md border border-slate-200/60 rounded-2xl p-5 hover:border-indigo-300 hover:shadow-xl hover:shadow-indigo-500/5 transition-all duration-300 cursor-pointer"
            >
              <div className="flex flex-col md:flex-row md:items-center justify-between gap-6">
                {/* Rota e Detalhes */}
                <div className="flex-1">
                  <div className="flex items-center gap-3 text-xs font-bold text-slate-400 uppercase tracking-widest mb-3">
                    <Clock size={14} />
                    {new Date(item.date).toLocaleDateString()}
                  </div>

                  <div className="flex items-center gap-4">
                    <div className="flex flex-col items-center gap-1">
                      <div className="w-2 h-2 rounded-full bg-indigo-500" />
                      <div className="w-[1px] h-4 bg-slate-200" />
                      <div className="w-2 h-2 rounded-full border-2 border-indigo-500" />
                    </div>

                    <div className="flex flex-col gap-1">
                      <span className="text-sm font-bold text-slate-700 leading-none">
                        {item.from}
                      </span>
                      <ArrowRight size={12} className="text-slate-300 my-1" />
                      <span className="text-sm font-bold text-slate-700 leading-none">
                        {item.to}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Preço e Status */}
                <div className="flex items-center justify-between md:justify-end md:gap-8 border-t md:border-t-0 border-slate-100 pt-4 md:pt-0">
                  <div className="text-right">
                    <p className="text-xs font-bold text-slate-400 uppercase">
                      {t("history.table.price")}
                    </p>
                    <p className="text-xl font-black text-slate-900">
                      {item.price}
                    </p>
                  </div>

                  <div className="h-10 w-10 rounded-full bg-slate-50 flex items-center justify-center text-slate-300 group-hover:bg-indigo-600 group-hover:text-white transition-all shadow-inner">
                    <ChevronRight size={20} />
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      ) : (
        /* Estado Vazio */
        <div className="text-center py-20 bg-slate-50/50 rounded-3xl border-2 border-dashed border-slate-200">
          <div className="inline-flex p-4 bg-white rounded-2xl shadow-sm mb-4">
            <Inbox className="text-slate-300" size={32} />
          </div>
          <p className="text-slate-500 font-medium">{t("history.empty")}</p>
        </div>
      )}
    </div>
  );
};

export default History;
