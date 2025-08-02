import { graphql, useLazyLoadQuery } from "react-relay";
import User from "./User.tsx";
import type { HomeQuery } from "./__generated__/HomeQuery.graphql.ts";

export default function Home() {
  const data = useLazyLoadQuery<HomeQuery>(
    graphql`
      query HomeQuery {
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
      <h1>Users</h1>
      {users?.map((user) => (
        <User key={user.id} user={user} />
      ))}
    </div>
  );
}