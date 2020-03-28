﻿using QuickFix;
using QuickFix.Fields;
using System;
using System.Collections.Generic;

namespace FixApp
{
    public class Executor : QuickFix.MessageCracker, QuickFix.IApplication
    {
        static readonly decimal DEFAULT_MARKET_PRICE = 10;

        int orderID = 0;
        int execID = 0;

        List<Session> _sessions = new List<Session>();

        private string GenOrderID() { return (++orderID).ToString(); }
        private string GenExecID() { return (++execID).ToString(); }

        #region QuickFix.Application Methods

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("IN:  " + message);
            Crack(message, sessionID);
            // SendMessage(message);
        }

        public void ToApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("OUT: " + message);
        }

        public void FromAdmin(Message message, SessionID sessionID) { }
        public void OnCreate(SessionID sessionID)
        {
            _sessions.Add(Session.LookupSession(sessionID));
        }
        public void OnLogout(SessionID sessionID) { }
        public void OnLogon(SessionID sessionID) { }
        public void ToAdmin(Message message, SessionID sessionID) { }
        #endregion

        private void SendMessage(Message m)
        {
            Console.WriteLine("SendMessage: "+ m);
            _sessions.ForEach(delegate (Session session)
            {
                Console.WriteLine("Session: " + session);
                session.Send(m);
            });
        }

        public void OnMessage(QuickFix.FIX44.NewOrderSingle n, SessionID s)
        {
            Symbol symbol = n.Symbol;
            Side side = n.Side;
            OrdType ordType = n.OrdType;
            OrderQty orderQty = n.OrderQty;
            Price price = new Price(DEFAULT_MARKET_PRICE);
            ClOrdID clOrdID = n.ClOrdID;

            switch (ordType.getValue())
            {
                case OrdType.LIMIT:
                    price = n.Price;
                    if (price.Obj == 0)
                        throw new IncorrectTagValue(price.Tag);
                    break;
                case OrdType.MARKET: break;
                default: throw new IncorrectTagValue(ordType.Tag);
            }

            QuickFix.FIX44.ExecutionReport exReport = new QuickFix.FIX44.ExecutionReport(
                new OrderID(GenOrderID()),
                new ExecID(GenExecID()),
                new ExecType(ExecType.FILL),
                new OrdStatus(OrdStatus.FILLED),
                symbol, //shouldn't be here?
                side,
                new LeavesQty(0),
                new CumQty(orderQty.getValue()),
                new AvgPx(price.getValue()));

            exReport.Set(clOrdID);
            exReport.Set(symbol);
            exReport.Set(orderQty);
            exReport.Set(new LastQty(orderQty.getValue()));
            exReport.Set(new LastPx(price.getValue()));

            if (n.IsSetAccount())
                exReport.SetField(n.Account);

            try
            {
                // Session.SendToTarget(exReport, s);
                SendMessage(exReport);
            }
            catch (SessionNotFound ex)
            {
                Console.WriteLine("==session not found exception!==");
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void OnMessage(QuickFix.FIX44.News n, SessionID s) { }

        public void OnMessage(QuickFix.FIX44.OrderCancelRequest msg, SessionID s)
        {
            string orderid = (msg.IsSetOrderID()) ? msg.OrderID.Obj : "unknown orderID";
            QuickFix.FIX44.OrderCancelReject ocj = new QuickFix.FIX44.OrderCancelReject(
                new OrderID(orderid), msg.ClOrdID, msg.OrigClOrdID, new OrdStatus(OrdStatus.REJECTED), new CxlRejResponseTo(CxlRejResponseTo.ORDER_CANCEL_REQUEST));
            ocj.CxlRejReason = new CxlRejReason(CxlRejReason.OTHER);
            ocj.Text = new Text("Executor does not support order cancels");

            try
            {
                // Session.SendToTarget(ocj, s);
                SendMessage(ocj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void OnMessage(QuickFix.FIX44.OrderCancelReplaceRequest msg, SessionID s)
        {
            string orderid = (msg.IsSetOrderID()) ? msg.OrderID.Obj : "unknown orderID";
            QuickFix.FIX44.OrderCancelReject ocj = new QuickFix.FIX44.OrderCancelReject(
                new OrderID(orderid), msg.ClOrdID, msg.OrigClOrdID, new OrdStatus(OrdStatus.REJECTED), new CxlRejResponseTo(CxlRejResponseTo.ORDER_CANCEL_REPLACE_REQUEST));
            ocj.CxlRejReason = new CxlRejReason(CxlRejReason.OTHER);
            ocj.Text = new Text("Executor does not support order cancel/replaces");

            try
            {
                // Session.SendToTarget(ocj, s);
                SendMessage(ocj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void OnMessage(QuickFix.FIX44.BusinessMessageReject n, SessionID s) { }

    }
}