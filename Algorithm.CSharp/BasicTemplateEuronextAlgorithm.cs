/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using QuantConnect.Data;
using QuantConnect.Brokerages;
using QuantConnect.Securities;
using QuantConnect.Securities.Equity;
using QuantConnect.Securities.Future;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// Basic template algorithm demonstrating how to subscribe to Euronext equities and index futures.
    /// Each Euronext venue is its own Lean market, with its own trading hours, holidays and quote currency.
    /// </summary>
    public class BasicTemplateEuronextAlgorithm : QCAlgorithm
    {
        private Equity _lvmh;
        private Equity _enel;
        private Equity _equinor;
        private Future _cac40;

        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        public override void Initialize()
        {
            SetStartDate(2025, 6, 2);
            SetEndDate(2025, 6, 27);

            SetAccountCurrency(Currencies.EUR);
            SetCash(1000000);
            // Euronext Oslo stocks are quoted in Norwegian krone
            SetCash(Currencies.NOK, 0);

            SetBrokerageModel(BrokerageName.InteractiveBrokersBrokerage, AccountType.Margin);

            _lvmh = AddEquity("MC", Resolution.Minute, Market.EuronextParis);
            _enel = AddEquity("ENEL", Resolution.Minute, Market.EuronextMilan);
            _equinor = AddEquity("EQNR", Resolution.Minute, Market.EuronextOslo);

            _cac40 = AddFuture(Futures.Indices.CAC40, Resolution.Minute, Market.EuronextParis,
                dataMappingMode: DataMappingMode.LastTradingDay);
            _cac40.SetFilter(TimeSpan.Zero, TimeSpan.FromDays(60));

            SetBenchmark(AddIndex("CAC40", Resolution.Minute).Symbol);
        }

        /// <summary>
        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// </summary>
        /// <param name="slice">Slice object keyed by symbol containing the stock data</param>
        public override void OnData(Slice slice)
        {
            if (Portfolio.Invested)
            {
                return;
            }

            if (slice.ContainsKey(_lvmh.Symbol) && slice.ContainsKey(_enel.Symbol) && slice.ContainsKey(_equinor.Symbol))
            {
                SetHoldings(_lvmh.Symbol, 0.25);
                SetHoldings(_enel.Symbol, 0.25);
                SetHoldings(_equinor.Symbol, 0.25);
            }

            if (_cac40.Mapped != null && slice.ContainsKey(_cac40.Mapped))
            {
                MarketOrder(_cac40.Mapped, 1);
            }
        }
    }
}
