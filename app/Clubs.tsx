import { graphql, useLazyLoadQuery } from "react-relay";
import type { ClubsQuery } from "./__generated__/ClubsQuery.graphql";


export default function Clubs() {
  const data = useLazyLoadQuery<ClubsQuery>(
    graphql`
      query ClubsQuery {
        getClubs {
          id,
          name
        }
      }
    `,
    {}
  );

  const clubs = data?.getClubs?.filter((club) => club != null);

  return (
    <div>
      <h1>Tanks</h1>
      {clubs?.map((club) => (
        <div>{club.name}</div>
      ))}
    </div>
  );
}