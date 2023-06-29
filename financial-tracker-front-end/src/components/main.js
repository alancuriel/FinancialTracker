"use client";
import React, { useState } from "react";

let pizzas = [
  {
    id: 1,
    name: "Cheese pizza",
    description: "very cheesy",
  },
  {
    id: 2,
    name: "Al Tono pizza",
    description: "lots of tuna",
  },
];

const Pizza = ({ pizza }) => {
  const [data, setData] = useState(pizza);
  const [dirty, setDirty] = useState(false);

  function update(value, fieldName, obj) {
    setData({ ...obj, [fieldName]: value });
    setDirty(true);
  }

  function onSave() {
    setDirty(false);
    // make rest call
  }

  return (
    <React.Fragment>
      <div className="flex w-full justify-center border-b border-gray-300 bg-gradient-to-b from-zinc-200 pb-6 pt-8 backdrop-blur-2xl dark:border-neutral-800 dark:bg-zinc-800/30 dark:from-inherit lg:static lg:w-auto  lg:rounded-xl lg:border lg:bg-gray-200 lg:p-4 lg:dark:bg-zinc-800/30">
        <h3>
          <input
            onChange={(evt) => update(evt.target.value, "name", data)}
            value={data.name}
          />
        </h3>
        <div>
          <input
            onChange={(evt) => update(evt.target.value, "description", data)}
            value={data.description}
          />
        </div>
        {dirty ? (
          <div>
            <button onClick={() => onSave()}>Save</button>
          </div>
        ) : null}
      </div>
    </React.Fragment>
  );
};

const Main = () => {
  const data = pizzas.map((pizza) => <Pizza key={pizza.id} pizza={pizza} />);

  return <React.Fragment>{data}</React.Fragment>;
};

export default Main;
