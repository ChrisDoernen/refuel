import { type AppQuery } from "./__generated__/AppQuery.graphql";
import { graphql, useLazyLoadQuery } from "react-relay";
import User from "./User.tsx";

export default function App() {
  const data = useLazyLoadQuery<AppQuery>(
    graphql`
      query AppQuery {
        getUsers {
          id
          ...User_item
        }
      }
    `,
    {}
  );

  const users = data?.getUsers?.filter((user) => user != null);

  return (
    <div>
      <h1>Star Wars Films</h1>
      {users?.map((user) => (
        <User key={user.id} user={user} />
      ))}
    </div>
  );
}