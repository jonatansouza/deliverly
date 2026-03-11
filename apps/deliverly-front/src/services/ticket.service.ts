import api from "../lib/api";
import type { Ticket, TicketRequest } from "../types/ticket";

export const ticketService = {
  create: (payload: TicketRequest): Promise<Ticket> =>
    api.post<Ticket>("/ticket", payload).then((res) => res.data),
};
